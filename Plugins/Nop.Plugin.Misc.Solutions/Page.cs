using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Core.Infrastructure;
using System;
using System.Text;


namespace Nop.Plugin.Misc.Solutions
{
    public class Page
    {
        public string TemplateType
        {
            get;
            set;
        }
        public string SystemName
        {
            get;
            set;
        }
        public string PageName
        {
            get;
            set;
        }
        public string Parent
        {
            get;
            set;
        }
        public string GrandParent
        {
            get;
            set;
        }
        public string GreatGrandParent
        {
            get;
            set;
        }
        public List<string> Siblings
        {
            get;
            set;
        }
        public List<string> ParentChildren
        {
            get;
            set;
        } //siblings plus me!
        public bool PageExists = true;

        private int segmentIndex
        {
            get;
            set;
        }
        private int numSegments
        {
            get;
            set;
        }
        private List<Topic> sortedTopics
        {
            get;
            set;
        }

        //perhaps add grandparents, parent siblings, and grandparent siblings
        /// <summary>
        /// Get detail about the current requested page from the stored topic. Remember template type is appended to Topic SystemName, but not the URL
        /// This accounts for matching the different array index when looking for a match
        /// </summary>
        /// <param name="segments">Array of url segments found in the request</param>
        /// Example SystemName for detail page: Solutions.EID-Software.Stock-Recorder.Sheep.Movements.Send-to-Slaughter.Detail
        /// URL segments for above System name: Solutions.EID-Software.Stock-Recorder.Sheep.Movements.Send-to-Slaughter
        /// 
        public Page(string[] segments)
        {
            //clean up and remove any null elements from array (not all requests use all segments)
            segments = segments.Where(s => s != null).ToArray();
            init(segments);
        }

        private void init(string[] segments)
        {
            //clean up and remove any null elements from array (not all requests use all segments)
            segments = segments.Where(s => s != null).ToArray();

            var systemNameFromPath = getSystemNamePathFromUrl(segments);

            //get a list of all topics
            var topicRepository = EngineContext.Current.Resolve<IRepository<Topic>>();
            var topics = topicRepository.Table.ToList();

            //get sorted Solutions Topics
            this.sortedTopics = topics.OrderBy(t => t.SystemName).Where(t => t.SystemName.Contains("Solutions.")).ToList();

            //get the topic which we are looking for
            var thisTopic = this.sortedTopics.Where(tt => (
                ancestorPath(tt.SystemName, 1).ToLower().Equals(systemNameFromPath.ToLower()) 
                ));

            if (thisTopic.FirstOrDefault() == null)
            {
                this.PageExists = false;

                return;
            }

            //split the found dot-delmited system-name into an array
            string[] tree = thisTopic.FirstOrDefault().SystemName.Split('.');

            this.TemplateType = tree.Last();
            this.numSegments = tree.Length;
            //this.PageName = segments.Last();
            this.PageName = tree[segments.Length - 1];
            this.segmentIndex = segments.Length;
            if (segments.Length > 1)
                this.Parent = segments[segments.Length - 2];
            if (segments.Length > 2)
                this.GrandParent = segments[segments.Length - 3];
            if (segments.Length > 3)
                this.GreatGrandParent = segments[segments.Length - 4];

            this.SystemName = thisTopic.FirstOrDefault().SystemName;
            //test
            string test = ancestorPath(this.SystemName, 1);

            //parent children
            var parentChildren = this.sortedTopics
             .Where(
              pc => (
               pc.SystemName.ToLower().Contains(ancestorPath(systemNameFromPath, 1).ToLower()) && //string match
               pc.SystemName.Split('.').Length == systemNameFromPath.Split('.').Length + 1 //SystemName length match
              )
             );
            List<string> parChild = new List<string>();
            foreach (Topic t in parentChildren)
            {
                tree = t.SystemName.Split('.');
                parChild.Add(tree[tree.Length - 2]);
            }
            this.ParentChildren = parChild;

            //get siblings (all parent children, but not me)
            var siblingTopics = parentChildren.Where(
             pc => (
              pc.SystemName.ToLower().Contains(ancestorPath(systemNameFromPath, 1).ToLower()) &&
              !pc.SystemName.ToLower().Equals(String.Format("{0}.{1}", systemNameFromPath.ToLower(), this.TemplateType.ToLower()))
             )
            );

            List<string> sib = new List<string>();
            foreach (Topic t in siblingTopics)
            {
                tree = t.SystemName.Split('.');
                sib.Add(tree[tree.Length - 2]);
            }
            this.Siblings = sib;
        }

        public List<string> ParentSiblings()
        {
            if (this.segmentIndex > 3) //has a parent
            {
                //get a list of our parent category siblings
                List<string> ps = new List<string>();
                foreach (Topic t in this.sortedTopics)
                {
                    var tree = t.SystemName.Split('.');
                    if (tree.Length >= this.segmentIndex)
                    {
                        if (tree[this.segmentIndex - 3].ToLower() == this.GrandParent.ToLower())
                        {
                            ps.Add(tree[this.segmentIndex - 2]);
                        }
                    }
                }
                //remove duplicates and our own parent from the list
                var distinctPs = new List<string>(ps.Distinct(StringComparer.InvariantCultureIgnoreCase));
                distinctPs.RemoveAt(distinctPs.IndexOf(this.Parent));

                return distinctPs;
            }

            return emptyList();
        }

        public List<string> GrandParentSiblings()
        {
            if (this.segmentIndex > 4) //has a grandparent
            {
                //get a list of our parent category siblings
                List<string> gps = new List<string>();
                foreach (Topic t in this.sortedTopics)
                {
                    var tree = t.SystemName.Split('.');
                    if (tree.Length >= this.segmentIndex)
                    {
                        if (tree[this.segmentIndex - 4].ToLower() == this.GreatGrandParent.ToLower())
                        {
                            gps.Add(tree[this.segmentIndex - 3]);
                        }
                    }
                }
                //remove duplicates and our own parent from the list
                var distinctGps = new List<string>(gps.Distinct(StringComparer.InvariantCultureIgnoreCase));
                distinctGps.RemoveAt(distinctGps.IndexOf(this.GrandParent));

                return distinctGps;
            }

            return emptyList();
        }

        public List<string> Children()
        {
            List<string> ch = new List<string>();
            foreach (Topic t in this.sortedTopics)
            {
                var tree = t.SystemName.Split('.');
                if (tree.Length > this.numSegments) //has children
                {
                    if (tree[this.segmentIndex - 1].ToLower() == this.PageName.ToLower())
                    {
                        ch.Add(tree[this.segmentIndex]);
                    }
                }
            }
            var distinctCh = new List<string>(ch.Distinct(StringComparer.InvariantCultureIgnoreCase));

            return distinctCh;
        }

        //must be a better way to do this, but for now...
        private List<string> emptyList()
        {
            List<string> empty = new List<string>();
            empty.Add(null);

            return empty;
        }

        private string getSystemNamePathFromUrl(string[] segments){
            StringBuilder sb = new StringBuilder("", 255);
            foreach (var segment in segments)
            {
                sb.Append(String.Format("{0}.", segment));
            }
            var systemNameFromPath = sb.ToString();//system name from path, excluding the template type

            return systemNameFromPath.Remove(systemNameFromPath.Length - 1); //remove the trailing dot
        }

        private string ancestorPath(string inputString, int levels)
        {
            List<string> l = inputString.Split('.').ToList();
            for (int i =levels; i > 0; i--)
            {
                l.Remove(l.Last());
            }

            return String.Join(".", l.ToArray());
        }
    }
}