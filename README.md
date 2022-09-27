# Applied
.NET Extension methods for object-object or dictionary-object or datatable-object mapping, single item mapping use [ item.Apply(()=>new { .. }); ],multiple array items mapping use [ items.Apply(a=>new { .. }); ]. 
<br/>Extension methods for SQL Window Function in Linq, use [ items.GroupBy(a =>...).AsPartition(p=> p.OrderBy(a=>...)).Over(p=>...); ],can also be customize function.

[![NuGet version (Applied)](https://img.shields.io/nuget/v/Applied)](https://www.nuget.org/packages/Applied/)

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
    

<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    public class TestData
    {
        public int Year { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal Sum { get; set; }
        public decimal Average { get; set; }
        public int RowNumber { get; set; }
        public int Ntile { get; set; }
        public int DenseRank { get; set; }
        public int Rank { get; set; }
        public decimal FirstValue { get; set; }
        public decimal LastValue { get; set; }
        public decimal NthValue { get; set; }
        public decimal Lead { get; set; }
        public decimal Lag { get; set; }
        public decimal CumeDist { get; set; }
        public decimal PercentRank { get; set; }
        public decimal PercentileDisc { get; set; }
        public decimal PercentileCont { get; set; }
        public decimal KeepDenseRankFirst { get; set; }
        public decimal KeepDenseRankLast { get; set; }
    }</code></pre>
    
<pre style="background-color: #eeeeee; border: 1px solid rgb(221, 221, 221); box-sizing: border-box; color: #333333; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 15px; line-height: 22px; margin-bottom: 22px; margin-top: 22px; max-width: 100%; overflow: auto; padding: 4.5px 11px;"><code class="language-cs hljs" style="background-attachment: initial; background-clip: initial; background-image: initial; background-origin: initial; background-position: initial; background-repeat: initial; background-size: initial; border-radius: 0px; border: none; display: block; font-family: &quot;Source Code Pro&quot;, Consolas, Courier, monospace; font-size: 1em; line-height: inherit; margin: 0px; overflow-x: auto; padding: 0px; text-size-adjust: none;">    List<TestData> data = new List<TestData>();
    data.Add(new TestData() { Year = 2019, Name = "A", Value = 111.1m });
    data.Add(new TestData() { Year = 2019, Name = "B", Value = 333.3m });
    data.Add(new TestData() { Year = 2019, Name = "C", Value = 333.3m });
    data.Add(new TestData() { Year = 2019, Name = "A", Value = 222.2m });
    data.Add(new TestData() { Year = 2019, Name = "C", Value = 444.4m });
    data.Add(new TestData() { Year = 2019, Name = "A", Value = 222.2m });
    data.Add(new TestData() { Year = 2019, Name = "B", Value = 333.3m });
    data.Add(new TestData() { Year = 2019, Name = "C", Value = 555.5m });
    data.Add(new TestData() { Year = 2020, Name = "A", Value = 111.1m });
    data.Add(new TestData() { Year = 2020, Name = "B", Value = 333.3m });
    data.Add(new TestData() { Year = 2020, Name = "A", Value = 222.2m });
    data.Add(new TestData() { Year = 2020, Name = "C", Value = 333.3m });

    data = data.GroupBy(a => new { a.Year }).AsPartition(p => p.OrderBy(a => a.Value).ThenBy(a => a.Name))
    .Over(p => p.Sum(a => a.Value), a => a.Sum)
    .Over(p => p.Average(a => a.Value), a => a.Average)
    .Over(p => p.RowNumber(), a => a.RowNumber)
    .Over(p => p.Ntile(2), a => a.Ntile)
    .Over(p => p.DenseRank(), a => a.DenseRank)
    .Over(p => p.Rank(), a => a.Rank)
    .Over(p => p.FirstValue(a => a.Value), a => a.FirstValue)
    .Over(p => p.LastValue(a => a.Value), a => a.LastValue)
    .Over(p => p.NthValue(a => a.Value, 2), a => a.NthValue)
    .Over(p => p.Lead(a => a.Value), a => a.Lead)
    .Over(p => p.Lag(a => a.Value), a => a.Lag)
    .Over(p => p.CumeDist(), a => a.CumeDist)
    .Over(p => p.PercentRank(), a => a.PercentRank)
    .Over(p => p.PercentileDisc(0.5m, a => a.Value), a => a.PercentileDisc)
    .Over(p => p.PercentileCont(0.5m, a => a.Value), a => a.PercentileCont)
    .Over(p => p.KeepDenseRankFirst(g => g.Sum(a => a.Value)), a => a.KeepDenseRankFirst)
    .Over(p => p.KeepDenseRankLast(g => g.Sum(a => a.Value)), a => a.KeepDenseRankLast)
    .ToList();</code></pre>
    

    
