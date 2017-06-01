# Applied
.NET Extension methods for object-object or dictionary-object or datatable-object mapping, single item mapping use [ item.Apply(()=>new { .. }); ],multiple array items mapping use [ items.Apply(a=>new { .. }); ]. 


<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">enum</span> UserEnum
    {
        None,
        User
    }
    [Serializable]
    <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">class</span> <span class="hljs-title" style="color: #a31515;">User</span>
    {
        <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">int</span> UserID { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">string</span> Name { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> DateTime? Time { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> UserEnum Enum { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
    }
    [Serializable]
    <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">class</span> <span class="hljs-title" style="color: #a31515;">UserViewModel</span>
    {
        <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">int</span> UserID { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> <span class="hljs-keyword" style="color: blue;">string</span> Name { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> DateTime? Time { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
        <span class="hljs-keyword" style="color: blue;">public</span> UserEnum Enum { <span class="hljs-keyword" style="color: blue;">get</span>; <span class="hljs-keyword" style="color: blue;">set</span>; }
    }</code></pre>
    
    
    
<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    User[] users = <span class="hljs-keyword" style="color: blue;">new</span> User[]
    {
        <span class="hljs-keyword" style="color: blue;">new</span> User() { UserID = <span class="hljs-number">1</span>, Name = <span class="hljs-string" style="color: #a31515;">"Sam     "</span> },
        <span class="hljs-keyword" style="color: blue;">new</span> User() { UserID = <span class="hljs-number">2</span>, Name = <span class="hljs-string" style="color: #a31515;">"John    "</span> }
    };

    users.Apply(a =&gt; <span class="hljs-keyword" style="color: blue;">new</span> { Time = DateTime.Now, Enum = UserEnum.User });
    users.Trim();</code></pre>

<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    users.Apply(a =&gt; <span class="hljs-keyword" style="color: blue;">new</span> { Time = DateTime.Now, Enum = UserEnum.User }).Trim();</code></pre>

<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    UserViewModel[] vm1 = users.ToDataEnumerable&lt;User, UserViewModel&gt;().ToArray();
    DataTable dt1 = users.ToDataTable();
    Dictionary&lt;<span class="hljs-keyword" style="color: blue;">string</span>, <span class="hljs-keyword" style="color: blue;">object</span>&gt;[] ds1 = users.ToDictionaries().ToArray();

    UserViewModel[] vm2 = dt1.ToDataEnumerable&lt;UserViewModel&gt;().ToArray();
    UserViewModel[] vm3 = ds1.ToDataEnumerable&lt;UserViewModel&gt;().ToArray();

    DataTable dt2 = vm1.ToDataTable();
    DataTable dt3 = ds1.ToDataTable();

    Dictionary&lt;<span class="hljs-keyword" style="color: blue;">string</span>, <span class="hljs-keyword" style="color: blue;">object</span>&gt;[] ds2 = vm1.ToDictionaries().ToArray();
    Dictionary&lt;<span class="hljs-keyword" style="color: blue;">string</span>, <span class="hljs-keyword" style="color: blue;">object</span>&gt;[] ds3 = dt1.ToDictionaries().ToArray();</code></pre>
    
    
    

    
<a href="https://www.nuget.org/packages/Applied/">https://www.nuget.org/packages/Applied/</a>
