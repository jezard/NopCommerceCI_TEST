# solutions-plugin (for V3.70)

## 
Notes:
Earlier versions of the plugin included theme modifications. These have been saved on the 'theme-demo' branch
The aim is now to keep the solutions plugin modular.


## Installation

- [x] Install and enable plugin in Administration panel
- [x] Ensure correct version of NopCommerce listed in Description.txt
- [x] Ensure any images uploaded to the plugin are also copied over
- [x] Add 'Solutions' topic and enable it to display in the top menu
- [x] See the Wiki page for how to generate the SQL inserts for the topics
- [x] Add TinyMCE Plugins 'solutionselements' and 'bootgrid' (\Presentation\Nop.Web\Content\tinymce\plugins) and copy over or amend init settings in \Presentation\Nop.Web\Administration\Views\Shared\EditorTemplates\RichEditor.cshtml
- [x] Roxy Fileman - update FILES_ROOT, "SESSION_PATH_KEY":    "DynamicDirectory", in \Presentation\Nop.Web\Content\Roxy_Fileman\conf.json
- [x] 'Unhack' the core code by restoring the default functionality in Presentation/Nop.Web/Administration/Controllers/RoxyFilemanController.cs Circa lines 141 and 151
- [x] Go to System -> Templates and + Add new record. _Name_ = Partial Template _View path_ = ~/Plugins/Misc.Solutions/Views/Solutions/shared/TopicPartial.cshtml. Need to make sure that id of solution topic is equal to the id of the correct Template in TopicTemplate table.  Can edit this query using the correct ID and Database:
```

 USE <DATABASE>
 UPDATE dbo.Topic SET TopicTemplateId = <ID> WHERE SystemName LIKE 'Solutions.%';
```



# solutions-plugin (for V3.80)

## 
Notes:
There are a few changes to note with version 3.80
- The TopicTable now contains a 'Published' Field (bit, not null), so if copying from topics from version 3.7 it may ne necessary to create the field and give it a default value of 1 so that the records can be inserted
- The path to Administration content path has changed from to \Presentation\Nop.Web\Administration\Content \Presentation\Nop.Web\Content which is probably enough of a reason to create two different versions of the plugin


## Installation

- [x] Install and enable plugin in Administration panel
- [x] Ensure correct version of NopCommerce listed in Description.txt
- [x] Ensure any images uploaded to the plugin are also copied over
- [x] Add 'Solutions' topic and enable it to display in the top menu
- [ ] See the Wiki page for how to generate the SQL inserts for the topics
- [x] Add TinyMCE Plugins 'solutionselements' and 'bootgrid' (\Presentation\Nop.Web\Administration\Content\tinymce\plugins) and copy over or amend init settings in \Presentation\Nop.Web\Administration\Views\Shared\EditorTemplates\RichEditor.cshtml
- [x] Roxy Fileman - update FILES_ROOT, "SESSION_PATH_KEY":    "DynamicDirectory", in \Presentation\Nop.Web\Administration\Content\Roxy_Fileman\conf.json
- [x] 'Unhack' the core code by restoring the default functionality in Presentation/Nop.Web/Administration/Controllers/RoxyFilemanController.cs Circa lines 141 and 151
- [x] Go to System -> Templates and + Add new record. _Name_ = Partial Template _View path_ = ~/Plugins/Misc.Solutions/Views/Solutions/shared/TopicPartial.cshtml. Need to make sure that id of solution topic is equal to the id of the correct Template in TopicTemplate table.  Can edit this query using the correct ID and Database:
```

 USE <DATABASE>
 UPDATE dbo.Topic SET TopicTemplateId = <ID> WHERE SystemName LIKE 'Solutions.%';
```
