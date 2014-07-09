<!DOCTYPE html>
<html>

<head>
<meta charset="utf-8" />
<title>{{>Title}}</title>

{{>Meta}}

<link href="/content/site.css")" rel="stylesheet" type="text/css" />
<script src="/content/scripts/jquery-1.5.1.min.js")" type="text/javascript"></script>
<script src="/content/scripts/modernizr-1.7.min.js")" type="text/javascript"></script>

<style>.success { color: green; }</style>

</head>

<body>

{{>Body}}

<p>Links to other test pages:</p>
<ul>
	<li><a href="/">Home</a></li>	
	<li><a href="/some-other-page">Some other page</a></li>	
	<li><a href="/sub-folder/">Sub folder</a></li>	
	<li><a href="/sub-folder/sub-folder-page">Sub folder page</a></li>	
	<li><a href="/some-custom-url">Custom url</a></li>	
	<li><a href="/markdown-test">Markdown test</a></li>	
	<li><a href="/some-bogus-page">404 Test</a></li>	
</ul>

</body>
</html>