{{<Meta}} 
<meta property="some-property" content="some value">{{/Meta}}

{{<Title}}Markdown test{{/Title}}

<h2>{{>Title}}</h2>

<markdown>
**Some markdown**
</markdown>

<markdown>
### Some more markdown ###
<p>This is a P tag</p>

<a href="/">Home</a>

adskjhdsa lkdsaj dsakjh dsakjh dsakjdsa kjhdsa kjhdsa kjhdsa kjdsahkjdsa kjdsah
adskjhdsa lkdsaj dsakjh dsakjh dsakjdsa kjhdsa kjhdsa kjhdsa kjdsahkjdsa kjdsah
adskjhdsa lkdsaj dsakjh dsakjh dsakjdsa kjhdsa kjhdsa kjhdsa kjdsahkjdsa kjdsah
adskjhdsa lkdsaj dsakjh dsakjh dsakjdsa kjhdsa kjhdsa kjhdsa kjdsahkjdsa kjdsah

</markdown>

<markdown>
## Some more markdown ##

PostOffice
==========

</markdown>

<markdown>

# Heading

<h1>THIS IS SOME HTML</h1>

PostOffice
==========

Handles [any messages][home] you want to send to [Xero](https://xero.com) users (or organisations), dealing with destination routing, templating, audit, user prefs, etc.

# PostOffice Setup

**Clone:** PostOffice, <br>
feature: integration

## Core-Databases 
**Clone:** Core-Databases <br>

1. Open `CoreDatabases.sln` <br>
2. Right click MyXeroDB  
3. Unload Project
4. Right click MyXeroDB
5. Edit MyXeroDB.sqlproj
6. Edit `<SqlCmdVariable Include="DeployDemoData"><DefaultValue>XXXXXXXX</DefaultValue>` <br> Change to: `<SqlCmdVariable Include="DeployDemoData"><DefaultValue>.\DemoData\Script.MasterDemoDeployScript.sql</DefaultValue>` <br>
https://jira.inside.xero.com/secure/attachment/17578/DeployDemoData-FIX.PNG <br>
(exactly as shown in screenshot) 
7. Right click MyXeroDB
8. Reload Project
9. Launch `Local.publish.xml`
10. Publish

https://jira.inside.xero.com/secure/attachment/17577/35Users.PNG
Connect to your local database, confirm that the MyXero database exists
Confirm that dbo.User has 35 rows.

**Add password to demo user:**

1. Copy the UserName: `demo00000000-0000-0000-0000-000000001212@test.xero.com`
2. Navigate to: http://login.web
3. Click "Forgot Password"
4. Enter email: `demo00000000-0000-0000-0000-000000001212@test.xero.com`
5. Connect to your local database, `MyXero > dbo.ForgottenPassword`
6. Copy the ForgottenPasswordID from the row containing the User: `demo00000000-0000-0000-0000-000000001212@test.xero.com`
7. Navigate to http://login.web/password/reset/{ForgottenPasswordID} 
8. Enter a new password

## Mandatory step: Setup Test Business/Personal Accounts

1. Navigate to http://login.web
2. Login with the user `demo00000000-0000-0000-0000-000000001212@test.xero.com`
3. On the MyXero dashboard:
4. **Create a new organisation:** "PostOfficeBusinessTest"
5. **Create a new personal account:** "PostOfficePersonalTest"

https://jira.inside.xero.com/secure/attachment/17579/OrgsSetup.PNG

## Elastic Search Setup
**Download and unzip from:** http://www.elasticsearch.org/download/

**Edit the file:** config/elasticsearch.yml <br>
**Set a unique cluster name and node name.** <br>
_Example:_ <br>
`cluster.name: PostOffice-dev-richardd`<br>
`node.name: "dev-richardd"`<br>

**Run:** `/bin/elasticsearch.bat`<br>

**Note:** If prompted with _JAVA_PATH_ variable not found,<br>
Right click Computer > Properties > Advanced System Settings > Environment Variables <br>
Under 'System Variables' click 'New...' <br>
**Variable name:** JAVA_PATH <br>
**Variable value:** (path to your JRE for example: `C:\Program Files\Java\jre8`)

## Run Tests
_Startup monogodb if not allready running_ <br>
_Startup elasticsearch if not allready running_ <br>

1. Open PostOffice.sln
2. Build the project.
3. Right click on `Xero.PostOffice.Tests > Run unit tests`

https://jira.inside.xero.com/secure/attachment/17580/win.PNG

[home]:https://xero.com

</markdown>