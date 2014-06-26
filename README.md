# About R7.Taskgen

R7.Taskgen is a task variants generator for use with IT learning courses, which uses student's 
name as a seed to combine randomly selected tasks in student-specific variant. 

# TODO

- [ ] General 
	- [ ] Generate ideas about task complexity / errors feedback

- [ ] Code
	- [ ] Remove hard-coded logic (book aliases, etc.)
	- [ ] Variable number of tasks per variant  	
	- [ ] Implement update notification service
	- [ ] Implement automatic DB update service
	- [ ] Fix [issues](https://github.com/vsoc-2014/R7.TaskGen/issues) 
	
- [ ] Testing
	- [ ] Develop behavioral tests and describe them in the [wiki](https://github.com/vsoc-2014/R7.TaskGen/wiki)
	- [ ] Perform behavioral tests and fill [issues](https://github.com/vsoc-2014/R7.TaskGen/issues) 
	- [ ] Add automatic tests using NUnit  
	
- [ ] UI	
	- [ ] Add options window
	- [ ] Switch through available DB in UI
	- [ ] Editor mode
	- [ ] Hide to system tray?
  
- [ ] DB
	- [ ] Script DB shema
	- [ ] Script sample DB data
		- [ ] Add tasks for at least "Real numbers and console IO" section to sample DB
    - [ ] DB culture, author, author email
	- [ ] DB security
	- [ ] Support for multiple DB
	- [ ] Add culture code field to all culture-dependant parts
			
- [ ] Editor
	- [ ] Tasks editor
	- [ ] Books / sections editor

- [ ] Internationalization
	- [ ] Refactor all culture-specific code	
	- [ ] Translate help.xml to english
	
- [ ] Linux version
	- [ ] Make Debian packaging script

- [ ] Windows version  
	- [ ] Implement DocBook XML => HTML transform with XSLT
	- [ ] Make [NSIS](http://nsis.sourceforge.net/Download)-based installer
	
- [ ] Documentation
	- [ ] Update application help
	- [ ] Create project documentation in the [wiki](https://github.com/vsoc-2014/R7.TaskGen/wiki)
	- [ ] Document all classes and methods in the code with XML comments
