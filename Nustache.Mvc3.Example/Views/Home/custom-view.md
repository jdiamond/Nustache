{{<Meta}}<meta property="some-property" content="some value">{{/Meta}}

{{<Title}}Home{{/Title}}

<h2>{{>Title}}</h2>

<p>Do unprefixed Model properties work => <span class="success">{{ DoModelPropertiesWork }}</span></p>

<p>Do partials work => </p> {{>_some-partial}} 

<p>Do shared partials work => {{>_shared-partial}} </p>

<p>Does HTML encoding work => <span class="success">{{ DoesHtmlEncodingWork }}</span> </p>

<p>Does Unescaping work => <span class="success">{{{ DoesHtmlEncodingWork }}}</span> </p>

<p>Does international character encoding work? <span class="success">{{ DoesInternationalCharacterEncodingWork }}</span></p>

<p>Does Russian character encoding work? <span class="success">{{ DoesRussianCharacterEncodingWork }}</span></p>

