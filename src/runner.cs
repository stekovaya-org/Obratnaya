using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

public static class Extension {
  public static string ToUnixTimeMilliseconds(this DateTime dt){
    return ((DateTimeOffset)dt).ToUnixTimeMilliseconds().ToString();
  }
  public static decimal Pow(this decimal T1,decimal T2){
    decimal T3 = T1;
    if(T2 == 0) return 1;
    for(int i = 1; i < T2; i++){
      T1 = T1 * T3;
    }
    return T1;
  }
  public static string Append(this string str,string srt){
    //str = str + srt + "\r\n";
    return str + srt + "\r\n";
  }
  public static string[] Ccut(this string str,string com){
    return Regex.Replace(str,com + " ([^\n]+)","$1").Split(new char[]{','});
  }
  public static void Underflow(this Stack stk,int u,int i,string line){
    if(stk.Count < u){
      Obratnaya.Error(i,line,"Stack underflow:");
    }
  }
}
public class Obratnaya {
  public static bool erx = false;
  public static bool erd = false;
  public static string erc = "";
  public static int slft = 0;
  public static string[] libraries = Data.Library.Libraries;
  public static bool Check(string[] listis,Type a){
    bool r = true;
    for(int i = 0; i < listis.Length; i++){
      try{
        a.GetMethod("Parse",new Type[]{typeof(string)}).Invoke(null,new object[]{listis[i]});
      }catch{
        r = false;
        break;
      }
    }
    return r;
  }
  public static bool Check(string str,Type type){
    bool r = true;
    try{
      type.GetMethod("Parse",new Type[]{typeof(string)}).Invoke(null,new object[]{str});
    }catch{
      r = false;
    }
    return r;
  }
  public static Hashtable Gen(string data){
    return Gen(data,"text");
  }
  public static Hashtable Gen(bool data){
    return Gen(data,"boolean");
  }
  public static Hashtable Gen(string data,string type){
    return new Hashtable{
      ["data"] = data,
      ["type"] = type
    };
  }
  public static Hashtable Gen(bool data,string type){
    return new Hashtable{
      ["data"] = (data ? "1" : "0"),
      ["type"] = type
    };
  }
  public static void Error(int i,string line,string content){
    erc = erc == "" ? "\x1b[1m:" + (i + 1 - slft) + ": \x1b[m\x1b[31m" + (content.EndsWith(":") ? content : content + ":") + "\x1b[m\r\n" + line : erc;
    if(erx) Console.Error.WriteLine(erc);
    erd = true;
    if(erx) Environment.Exit(1);
  }
  public static void Error(string content){
    erc = erc == "" ? "\x1b[1m:FATAL: \x1b[m\x1b[31m" + (content.EndsWith(":") ? content : content + ":") + "\x1b[m" : erc;
    if(erx) Console.Error.WriteLine(erc);
    erd = true;
    if(erx) Environment.Exit(1);
  }
  public static void Run(string code){
    Run(code,false);
  }
  public static void Run(string code,bool ere){
    erx = ere;
    erd = false;
    slft = 0;
    for(int lcn = 0; lcn < libraries.Length; lcn++){
      if(!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "lib" + Path.DirectorySeparatorChar.ToString() + libraries[lcn].Replace("\\",Path.DirectorySeparatorChar.ToString()))){
        Error("Missing library: [" + libraries[lcn] + "], URL: [\x1b[m https://github.com/stekovaya-org/Obratnaya/blob/master/lib/" + libraries[lcn] + "/ \x1b[31m]");if(!erx){return;}
      }
    }
    var dstk = new System.Collections.Generic.List<Hashtable>();
    var secl = new System.Collections.Generic.Dictionary<string,int[]>();
    var varh = new Hashtable{
      ["pi"] = Gen(Math.PI.ToString(),"decimal"),
      ["e"] = Gen(Math.E.ToString(),"decimal"),
      ["version"] = Gen(Data.Version.FullVersion,"text"),
      ["version_major"] = Gen(Data.Version.Major,"decimal"),
      ["version_minor"] = Gen(Data.Version.Minor,"decimal"),
      ["version_patch"] = Gen(Data.Version.Patch,"decimal"),
      ["version_prefix"] = Gen(Data.Version.Prefix,"text"),
      ["version_suffix"] = Gen(Data.Version.Suffix,"text"),
      ["license"] = Gen(Data.License.LegalCode,"text"),
      ["license_name"] = Gen(Data.License.Name,"text"),
      ["local_0"] = Gen(((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds().ToString(),"decimal"),
      ["utc_0"] = Gen(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds().ToString(),"decimal")
    };
    var stk = new Stack();
    string sec = "";
    bool empo = false;
    string[] all = code.Replace("\r","").Split(new char[]{'\n'});
    AppDomain tAppDomain = Thread.GetDomain();
    AssemblyName tAssemblyName = new AssemblyName();
    tAssemblyName.Name = "a";
    AssemblyBuilder tAssembly = tAppDomain.DefineDynamicAssembly(tAssemblyName, AssemblyBuilderAccess.Save);
    ModuleBuilder tModule;
    tModule = tAssembly.DefineDynamicModule("aModule", "aModule.mod");
    TypeBuilder tDynamicClass = tModule.DefineType("aClass", TypeAttributes.Public);
    MethodBuilder tMainMethod = tDynamicClass.DefineMethod("Main", MethodAttributes.Public | MethodAttributes.Static, typeof(void), new Type[] { typeof(string[]) });
    ILGenerator IL = tMainMethod.GetILGenerator();
    bool rb = false;
    var cs = new System.Collections.Generic.List<string>();
    var ns = new System.Collections.Generic.List<int>();
    string _c = @"using System;
using System.Collections;

class MainClass {
  public static Hashtable Gen(string data,string type){
    return new Hashtable{
      [""data""] = data,
      [""type""] = type
    };
  }
  public static void Main(string[] args){
    decimal[] _da = {0,0};
    string[] _sa = {"""",""""};
    var _vt = new Hashtable{
      [""pi""] = Gen(Math.PI.ToString(),""decimal""),
      [""e""] = Gen(Math.E.ToString(),""decimal"")
    };
    var _d = new System.Collections.Generic.List<Hashtable>();
    var _s = new Stack();
";
    var print = typeof(Console).GetMethod("WriteLine", new Type[]{ typeof(string) });
    if(Regex.Matches(code,@"/\*").Count != Regex.Matches(code,@"\*/").Count) Error("Unmatched comment bracket:");
    loadsf:
    for(int i = 0; i < all.Length; i++){
      //Console.WriteLine(i.ToString() + ":" + (cs.Count != 0 ? cs[cs.Count - 1] : ""));
      varh["local_1"] = Gen(DateTime.Now.ToUnixTimeMilliseconds(),"decimal");
      varh["utc_1"] = Gen(DateTime.UtcNow.ToUnixTimeMilliseconds(),"decimal");
      varh["line"] = Gen((i + 1).ToString(),"decimal");
      string aline = all[i];
      string[] line = aline.Split(" ");
      if(cs.Count != 0){
      if(rb && i == secl[cs[cs.Count - 1]][1] + 1){
        if(cs.Count == 1){
        //Console.WriteLine(2);
        i = secl[cs[0]][2];
        sec = "main";
        rb = false;
        cs.RemoveAt(0);
        ns.RemoveAt(0);
        }else{
        //Console.WriteLine(1);
        var _bs = cs[cs.Count - 1];
        cs.RemoveAt(cs.Count - 1);
        ns.RemoveAt(cs.Count - 1);
        i = secl[_bs][2];
        }
        continue;
      }
      }
      if(line[0] == ".main:"){
        sec = "main";
        continue;
      }else if(line[0] == ".data:"){
        sec = "data";
        string ln;
        if(line.Length <= 1){
          ln = "100000";
        }else{
          ln = line[1];
        }
        if(!Check(ln,typeof(uint))) Error(i,aline,"Data size must be positive integer:");
        IL.Emit(OpCodes.Ldc_I4,int.Parse(ln));
        IL.Emit(OpCodes.Newarr,typeof(object));
        continue;
      }else if(line[0].StartsWith("_") && line[0].EndsWith(":") && sec != "main"){
        sec = Regex.Replace(line[0],@"_([^:]+):","$1");
        secl[sec] = new int[]{i,0,0};
        continue;
      }else if(line[0] == "#library" && line.Length >= 2){
        string path = "";
        for(int ia = 1; ia < line.Length; ia++){
          path+=(path == "" ? "" : " ") + line[ia];
        }
        var oldpath = path;
        var sep = Path.DirectorySeparatorChar.ToString();
        if(path.StartsWith("<") && path.EndsWith(">")) path = AppDomain.CurrentDomain.BaseDirectory + "lib" + sep + Regex.Replace(path,"<([^<>]+)>","$1");
        if(File.Exists(path)){
          slft += File.ReadAllText(path).Split(new char[]{'\n'}).Length - 1;
          all = (File.ReadAllText(path) + "\n" + Regex.Replace(code.Replace("\r",""),@"#library " + oldpath + "\n","")).Split(new char[]{'\n'});
          code = File.ReadAllText(path) + "\n" + Regex.Replace(code.Replace("\r",""),@"#library " + oldpath + "\n","");
          goto loadsf;
        }else{
          Error(i,aline,"File not found:");
        }
      }else if(line[0] == "#include" && line.Length >= 2){
        string path = "";
        for(int ia = 1; ia < line.Length; ia++){
          path+=(path == "" ? "" : " ") + line[ia];
        }
        var oldpath = path;
        var sep = Path.DirectorySeparatorChar.ToString();
        if(path.StartsWith("<") && path.EndsWith(">")) path = AppDomain.CurrentDomain.BaseDirectory + "lib" + sep + Regex.Replace(path,"<([^<>]+)>","$1");
        if(File.Exists(path)){
          slft += File.ReadAllText(path).Split(new char[]{'\n'}).Length - 1;
          all = (File.ReadAllText(path) + "\n" + Regex.Replace(code.Replace("\r",""),@"#include " + oldpath + "\n","")).Split(new char[]{'\n'});
          code = File.ReadAllText(path) + "\n" + Regex.Replace(code.Replace("\r",""),@"#include " + oldpath + "\n","");
          goto loadsf;
        }else{
          Error(i,aline,"File not found:");
        }
      }else if(line[0].StartsWith(";")){
        continue;
      }
      if(sec == "data"){
        string rp = Regex.Replace(aline,"^(?:\t|    )([^\n]+)$","$1");
        if(rp.StartsWith(";")){
          continue;
        }else if(Regex.IsMatch(rp,@"^\.text[ \t]+")){
          string rpl = Regex.Replace(Regex.Replace(rp,@"^\.text[ \t]+([^\n]+)$","$1"),@"\$<([0123456789ABCDEFabcdef]+)>",m=>(Convert.ToChar(Convert.ToInt32(m.Groups[1].ToString(),16)).ToString()).ToString());
          IL.Emit(OpCodes.Ldstr,rpl);
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem,typeof(string));
          dstk.Add(Gen(rpl,"text"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl.Replace("\"","\\\"") + "\",\"text\"));");
        }else if(Regex.IsMatch(rp,@"^\.decimal[ \t]+")){
          string rpl = Regex.Replace(rp,@"^\.decimal[ \t]+([^\n]+)$","$1");
          if(!Check(rpl,typeof(decimal))) Error(i,aline,"Cannot convert text to decimal:");
          IL.Emit(OpCodes.Ldc_R4,Single.Parse(rpl));
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem_R4);
          dstk.Add(Gen(rpl,"decimal"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl + "\",\"decimal\"));");
        }else if(Regex.IsMatch(rp,@"^\.boolean[ \t]+")){
          string rpl = Regex.Replace(rp,@"^\.boolean[ \t]+([^\n]+)$","$1");
          if(rpl != "0" && rpl != "1") Error(i,aline,"Cannot convert to boolean:");
          IL.Emit(OpCodes.Ldc_I4,int.Parse(rpl));
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem_I4);
          dstk.Add(Gen(rpl,"boolean"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl + "\",\"boolean\"));");
        }else if(rp == ".null"){
          IL.Emit(OpCodes.Ldstr,"");
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem,typeof(string));
          dstk.Add(Gen("","null"));
          _c = _c.Append("    _d.Add(Gen(\"\",\"null\"));");
        }else if(rp == ".empty"){
          IL.Emit(OpCodes.Ldstr,"");
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem,typeof(string));
          dstk.Add(Gen("","text"));
          _c = _c.Append("    _d.Add(Gen(\"\",\"text\"));");
        }else{
          Error(i,aline,"Unknown prefix:");
        }
      }else if(sec == "main"){
        string rp = Regex.Replace(aline,"^(?:\t|    )([^\n]+)$","$1");
        if(string.IsNullOrWhiteSpace(aline)){
          continue;
        }
        if(!aline.StartsWith("\t") && !aline.StartsWith("    ")){
          Error(i,aline,"Illegal indent detected");
        }
        if(rp.StartsWith("/*")) empo = true;
        if(empo && !rp.EndsWith("*/")) continue;
        if(rp.EndsWith("*/")){
          empo = false;
          continue;
        }
        rp = Regex.Replace(rp,@"^([^ ]+)[ \t]+","$1 ");
        if(rp.StartsWith(";")){
          continue;
        }else if(rp.StartsWith("len ")){
          string[] cp = rp.Ccut("len");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          string[] arr = {""};
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            arr[0] = ((Hashtable)stk.Pop())["data"].ToString();
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen(arr[0].Length.ToString(),"decimal"));
        }else if(rp.StartsWith("call ")){
          string[] cp = rp.Ccut("call");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          var dfs = (Hashtable)stk.Pop();
          if(dfs["type"].ToString() != "text") Error(i,aline,"Cannot call section by not text:");
          if(cp[0] != "@"){
            Error(i,aline,"Unknown format:");
          }
          if(secl.ContainsKey(dfs["data"].ToString())){
            secl[dfs["data"].ToString()][2] = i;
            i = secl[dfs["data"].ToString()][0];
            rb = true;
            cs.Add(dfs["data"].ToString());
            ns.Add(i);
            continue;
          }else{
            Error(i,aline,"Section not found:");
          }
        }else if(rp.StartsWith("cond ")){
          string[] cp = rp.Ccut("cond");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          stk.Underflow(1,i,aline);
          var dfs = (Hashtable)stk.Pop();
          if(dfs["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean:");
          string[] arr = {""};
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            var df = (Hashtable)stk.Pop();
            if(!Check(df["data"].ToString(),typeof(uint))) Error(i,aline,"Cannot jump to line by not positive integer:");
            arr[0] = df["data"].ToString();
          }else if(Check(cp[0],typeof(uint))){
            arr[0] = cp[0];
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(dfs["data"].ToString() == "1"){
            i = int.Parse(arr[0]) - 2 + (ns.Count == 0 ? slft : 0);
            //Console.WriteLine(i + 2);
            //Console.WriteLine(ns.Count);
            continue;
          }
        }else if(rp.StartsWith("get ")){
          string[] cp = rp.Ccut("get");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          string[] arr = {""};
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            arr[0] = ((Hashtable)stk.Pop())["data"].ToString();
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
          }else if(cp[0] == "%line"){
            arr[0] = "line";
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!varh.ContainsKey(arr[0])) Error(i,aline,"Undefined variable:");
          stk.Push((Hashtable)varh[arr[0]]);
        }else if(rp.StartsWith("define ")){
          string[] cp = rp.Ccut("define");
          string[] arr = {"",""};
          string[] tar = {"",""};
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            var dc = stk.Pop();
            arr[0] = ((Hashtable)dc)["data"].ToString();
            tar[0] = ((Hashtable)dc)["type"].ToString();
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
            tar[0] = "decimal";
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            var dc = stk.Pop();
            arr[1] = ((Hashtable)dc)["data"].ToString();
            tar[1] = ((Hashtable)dc)["data"].ToString();
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = cp[1];
            tar[1] = "decimal";
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[0] == cp[1] && cp[0] == "@"){
            arr = arr.Reverse().ToArray();
            tar = tar.Reverse().ToArray();
          }
          if(varh.ContainsKey(arr[0])){
            varh[arr[0]] = Gen(arr[1],tar[1]);
          }else{
            varh.Add(arr[0],Gen(arr[1],tar[1]));
          }
        }else if(rp.StartsWith("< ")){
          string[] cp = rp.Ccut("<");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1] < arr[0],"boolean"));
        }else if(rp.StartsWith("> ")){
          string[] cp = rp.Ccut(">");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1] > arr[0],"boolean"));
        }else if(rp.StartsWith("<= ")){
          string[] cp = rp.Ccut("<=");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1] <= arr[0],"boolean"));
        }else if(rp.StartsWith(">= ")){
          string[] cp = rp.Ccut(">=");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot compare not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1] >= arr[0],"boolean"));
        }else if(rp.StartsWith("= ")){
          string[] cp = rp.Ccut("=");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[0] = s["data"].ToString();
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[1] = s["data"].ToString();
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = cp[1];
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1] == arr[0],"boolean"));
        }else if(rp.StartsWith("== ")){
          string[] cp = rp.Ccut("==");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          string[] art = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[0] = s["data"].ToString();
            art[0] = s["type"].ToString();
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
            art[0] = "decimal";
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[1] = s["data"].ToString();
            art[1] = s["type"].ToString();
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = cp[1];
            art[1] = "decimal";
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen((arr[1] == arr[0] && art[1] == art[0]),"boolean"));
        }else if(rp.StartsWith("mov ")){
          string[] cp = rp.Ccut("mov");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          if(!Check(cp[0],typeof(uint))) Error(i,aline,"Cannot access to stack by not positive integer:");
          if(dstk.Count <= int.Parse(cp[0])) Error(i,aline,"Stack underflow:");
          if(cp[1] == "@"){
            stk.Push((Hashtable)dstk[int.Parse(cp[0])]);
            _c = _c.Append("    _s.Push((Hashtable)_d[" + cp[0] + "]);");
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp.StartsWith("movb ")){
          string[] cp = rp.Ccut("movb");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          if(!Check(cp[1],typeof(uint))) Error(i,aline,"Cannot access to stack by not positive integer:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            if(dstk.Count - 1 < int.Parse(cp[1])){
              for(int n = 0; n < int.Parse(cp[1]) - dstk.Count + 2; n++){
                dstk.Add(Gen("0","decimal"));
              }
            }
            string h = ((Hashtable)stk.Pop())["data"].ToString();
            if(h != "1" && h != "0") Error(i,aline,"Cannot convert to boolean");
            dstk[int.Parse(cp[1])] = Gen(h,"boolean");
            _c = _c.Append("    if(_d.Count - 1 < int.Parse(\"" + cp[1] + "\")){\r\n      for(int n = 0; n < int.Parse(\"" + cp[1] + "\") - _d.Count + 2; n++){\r\n        _d.Add(Gen(\"0\",\"decimal\"));\r\n      }\r\n    }\r\n    _d[" + cp[1] + "] = Gen(((Hashtable)_s.Pop())[\"data\"].ToString(),\"boolean\");");
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp.StartsWith("movd ")){
          string[] cp = rp.Ccut("movd");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          if(!Check(cp[1],typeof(uint))) Error(i,aline,"Cannot access to stack by not positive integer:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            if(dstk.Count - 1 < int.Parse(cp[1])){
              for(int n = 0; n < int.Parse(cp[1]) - dstk.Count + 2; n++){
                dstk.Add(Gen("0","decimal"));
              }
            }
            string h = ((Hashtable)stk.Pop())["data"].ToString();
            if(!Check(h,typeof(decimal))) Error(i,aline,"Cannot convert to decimal");
            dstk[int.Parse(cp[1])] = Gen(h,"decimal");
            _c = _c.Append("    if(_d.Count - 1 < int.Parse(\"" + cp[1] + "\")){\r\n      for(int n = 0; n < int.Parse(\"" + cp[1] + "\") - _d.Count + 2; n++){\r\n        _d.Add(Gen(\"0\",\"decimal\"));\r\n      }\r\n    }\r\n    _d[" + cp[1] + "] = Gen(((Hashtable)_s.Pop())[\"data\"].ToString(),\"decimal\");");
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp.StartsWith("movt ")){
          string[] cp = rp.Ccut("movt");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          if(!Check(cp[1],typeof(uint))) Error(i,aline,"Cannot access to stack by not positive integer:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            if(dstk.Count - 1 < int.Parse(cp[1])){
              for(int n = 0; n < int.Parse(cp[1]) - dstk.Count + 2; n++){
                dstk.Add(Gen("0","decimal"));
              }
            }
            dstk[int.Parse(cp[1])] = Gen(((Hashtable)stk.Pop())["data"].ToString(),"text");
            _c = _c.Append("    if(_d.Count - 1 < int.Parse(\"" + cp[1] + "\")){\r\n      for(int n = 0; n < int.Parse(\"" + cp[1] + "\") - _d.Count + 2; n++){\r\n        _d.Add(Gen(\"0\",\"decimal\"));\r\n      }\r\n    }\r\n    _d[" + cp[1] + "] = Gen(((Hashtable)_s.Pop())[\"data\"].ToString(),\"text\");");
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp == "clear"){
          Console.Clear();
          _c = _c.Append("    Console.Clear();");
        }else if(rp == "cr"){
          Console.Write("\r");
          _c = _c.Append("    Console.Write(\"\\r\");");
        }else if(rp == "lf"){
          Console.Write("\n");
          _c = _c.Append("    Console.Write(\"\\n\");");
        }else if(rp.StartsWith("msg ")){
          string[] cp = rp.Ccut("msg");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            Console.Write(((Hashtable)stk.Pop())["data"].ToString());
            IL.EmitCall(OpCodes.Call,print,new Type[]{ typeof(string) });
            _c = _c.Append("    Console.Write(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp.StartsWith("pow ")){
          string[] cp = rp.Ccut("pow");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot pow not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = Convert.ToDecimal(((Hashtable)_s.Pop())[\"data\"]);");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot pow not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = Convert.ToDecimal(((Hashtable)_s.Pop())[\"data\"]);");
            }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[1].Pow(arr[0]).ToString(),"decimal"));
          //_c = _c.Append("    _s.Push(Gen((_da[0] + _da[1]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("add ")){
          string[] cp = rp.Ccut("add");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot add not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = Convert.ToDecimal(((Hashtable)_s.Pop())[\"data\"]);");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot add not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = Convert.ToDecimal(((Hashtable)_s.Pop())[\"data\"]);");
            }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] + arr[1]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[0] + _da[1]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("sub ")){
          string[] cp = rp.Ccut("sub");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot subtract not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[1] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot subtract not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[0] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen((arr[1] - arr[0]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[1] - _da[0]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("mul ")){
          string[] cp = rp.Ccut("mul");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot multiply not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot multiply not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] * arr[1]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[0] * _da[1]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("div ")){
          string[] cp = rp.Ccut("div");
          if(cp.Length != 2 && cp.Length != 3) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot divide not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[1] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot divide not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[0] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(!(cp[0] == cp[1] && cp[0] == "@")) arr = arr.Reverse().ToArray();
          stk.Push(Gen((cp.Length == 3 ? (cp[2] == "1" ? Math.Floor(arr[1] / arr[0]) : (arr[1] / arr[0])) : arr[1] / arr[0]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[1] / _da[0]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("mod ")){
          string[] cp = rp.Ccut("mod");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot mod not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot mod not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = decimal.Parse(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[1] % arr[0]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[1] % _da[0]).ToString(),\"decimal\"));");
        }else if(rp.StartsWith("and ")){
          string[] cp = rp.Ccut("and");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[0] = s["data"].ToString();
            _c = _c.Append("    _sa[0] = ((Hashtable)_s.Pop())[\"data\"].ToString();");
          }else if(cp[0] == "1" || cp[0] == "0"){
            arr[0] = cp[0];
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[1] = s["data"].ToString();
            _c = _c.Append("    _sa[1] = ((Hashtable)_s.Pop())[\"data\"].ToString();");
          }else if(cp[1] == "1" || cp[1] == "0"){
            arr[1] = cp[1];
            _c = _c.Append("    _sa[1] = \"" + cp[1] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] == "1" && arr[1] == "1" ? "1" : "0"),"boolean"));
          _c = _c.Append("    _s.Push(Gen((_sa[0] == \"1\" && _sa[1] == \"1\" ? \"1\" : \"0\"),\"boolean\"));");
        }else if(rp.StartsWith("or ")){
          string[] cp = rp.Ccut("or");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[0] = s["data"].ToString();
            _c = _c.Append("    _sa[0] = (string)((Hashtable)_s.Pop())[\"data\"];");
          }else if(cp[0] == "1" || cp[0] == "0"){
            arr[0] = cp[0];
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[1] = s["data"].ToString();
            _c = _c.Append("    _sa[1] = (string)((Hashtable)_s.Pop())[\"data\"];");
          }else if(cp[1] == "1" || cp[1] == "0"){
            arr[1] = cp[1];
            _c = _c.Append("    _sa[1] = \"" + cp[1] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] == "1" || arr[1] == "1" ? "1" : "0"),"boolean"));
          _c = _c.Append("    _s.Push(Gen((_sa[0] == \"1\" || _sa[1] == \"1\" ? \"1\" : \"0\"),\"boolean\"));");
        }else if(rp.StartsWith("xor ")){
          string[] cp = rp.Ccut("xor");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[0] = s["data"].ToString();
            _c = _c.Append("    _sa[0] = (string)((Hashtable)_s.Pop())[\"data\"];");
          }else if(cp[0] == "1" || cp[0] == "0"){
            arr[0] = cp[0];
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean(s):");
            arr[1] = s["data"].ToString();
            _c = _c.Append("    _sa[1] = (string)((Hashtable)_s.Pop())[\"data\"];");
          }else if(cp[1] == "1" || cp[1] == "0"){
            arr[1] = cp[1];
            _c = _c.Append("    _sa[1] = \"" + cp[1] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] == "1" ^ arr[1] == "1" ? "1" : "0"),"boolean"));
          _c = _c.Append("    _s.Push(Gen((_sa[0] == \"1\" ^ _sa[1] == \"1\" ? \"1\" : \"0\"),\"boolean\"));");
        }else if(rp.StartsWith("not ")){
          string[] cp = rp.Ccut("not");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          string[] arr = {""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "boolean") Error(i,aline,"Cannot process not boolean:");
            arr[0] = s["data"].ToString();
            _c = _c.Append("    _sa[0] = (string)((Hashtable)_s.Pop())[\"data\"];");
          }else if(cp[0] == "1" || cp[0] == "0"){
            arr[0] = cp[0];
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] != "1" ? "1" : "0"),"boolean"));
          _c = _c.Append("    _s.Push(Gen((_sa[0] != \"1\" ? \"1\" : \"0\"),\"boolean\"));");
        }else if(rp.StartsWith("remove ")){
          string[] cp = rp.Ccut("remove");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          int[] arr = {0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal" || !Check(s["data"].ToString(),typeof(uint))) Error(i,aline,"Cannot process not positive integer:");
            arr[0] = int.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[0],typeof(uint))){
            arr[0] = int.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(arr[0] >= dstk.Count) Error(i,aline,"Stack underflow:");
          dstk.RemoveAt(arr[0]);
          _c = _c.Append("    _d.RemoveAt((int)_da[0]);");
        }else if(rp == "ret"){
          _c = _c.Append("    Environment.Exit(0);");
          //if(File.Exists("out.cs")) File.Delete("out.cs");
          //File.WriteAllText("out.cs","");
          //File.WriteAllText("out.cs",_c + "  }\r\n}");
          Environment.Exit(0);
        }else if(rp.StartsWith("ret ")){
          string[] cp = rp.Ccut("ret");
          int intv = 0;
          if(cp.Length != 1)  Error(i,aline,"Arguments must be 1:");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            Hashtable s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot exit by not integer:");
            if(!Check(s["data"].ToString(),typeof(int))) Error(i,aline,"Cannot exit by not integer:");
            intv = int.Parse(s["data"].ToString());
            _c = _c.Append("    _sa[0] = ((Hashtable)_s.Pop())[\"data\"].ToString();");
          }else if(Check(cp[0],typeof(int))){
            intv = int.Parse(cp[0]);
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          _c = _c.Append("    Environment.Exit(int.Parse(_sa[0]));");
          //if(File.Exists("out.cs")) File.Delete("out.cs");
          //File.WriteAllText("out.cs","");
          //File.WriteAllText("out.cs",_c + "  }\r\n}");
          Environment.Exit(intv);
        }else if(rp.StartsWith("jmp ")){
          string[] cp = rp.Ccut("jmp");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 2:");
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot jump by not positive integer:");
            if(!Check(s["data"].ToString(),typeof(uint))) Error(i,aline,"Cannot jump by not positive integer:");
            i = int.Parse(s["data"].ToString()) - 2 + (ns.Count == 0 ? slft : 0);
          }else if(Check(cp[0],typeof(uint))){
            i = int.Parse(cp[0]) - 2 + (ns.Count == 0 ? slft : 0);
          }else{
            Error(i,aline,"Unknown format:");
          }
        }else if(rp == "pop"){
          stk.Underflow(1,i,aline);
          stk.Pop();
          _c = _c.Append("    _s.Pop();");
        }else if(rp == "dup"){
          stk.Underflow(1,i,aline);
          stk.Push((Hashtable)stk.Peek());
          _c = _c.Append("    _s.Push((Hashtable)_s.Peek());");
        }else if(rp.StartsWith("concat ")){
          string[] cp = rp.Ccut("concat");
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          string[] arr = {"",""};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[0] = s["data"].ToString();
            _c = _c.Append("    _sa[0] = ((Hashtable)_s.Pop())[\"data\"].ToString();");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = cp[0];
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            arr[1] = s["data"].ToString();
            _c = _c.Append("    _sa[1] = ((Hashtable)_s.Pop())[\"data\"].ToString();");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = cp[1];
            _c = _c.Append("    _sa[1] = \"" + cp[1] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[0] == cp[1] && cp[0] == "@") arr = arr.Reverse().ToArray();
          stk.Push(Gen(arr[0] + arr[1],"text"));
          _c = _c.Append("    _s.Push(Gen(_sa[1] + _sa[0],\"text\"));");
        }else if(rp.StartsWith("emit ")){
          string[] cp = rp.Ccut("emit");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          Hashtable sd = Gen("","");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            sd = (Hashtable)stk.Pop();
            if(!Check(cp[0],typeof(uint))) Error(i,aline,"Cannot emit text by not positive integer:");
            sd["type"] = "text";
            sd["data"] = Convert.ToChar(int.Parse(sd["data"].ToString())).ToString();
            _c = _c.Append("    _da[0] = (decimal)(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(uint))){
            sd = Gen(Convert.ToChar(int.Parse(cp[0])).ToString(),"text");
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(sd);
          _c = _c.Append("    _s.Push(Gen(Convert.ToString(Convert.ToChar(int.Parse(_da[0].ToString()))),\"text\"));");
        }else if(rp == "nop" || rp.StartsWith("nop ")){
        }else if(rp.StartsWith("push ")){
          string[] cp = rp.Ccut("push");
          if(cp.Length == 0) Error(i,aline,"Arguments must be 1≦:");
          if(cp.Length != 1){
            for(int ia = 1; ia < cp.Length; ia++){
              cp[0]+="," + cp[ia];
            }
          }
          Hashtable sd = Gen("","");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            sd = (Hashtable)stk.Pop();
            sd["type"] = "text";
            sd["data"] = sd["data"].ToString();
            _c = _c.Append("    _sa[0] = (string)(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            sd = Gen(cp[0],"text");
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            sd = Gen(cp[0],"text");
          }
          stk.Push(sd);
          _c = _c.Append("    _s.Push(Gen(_sa[0],\"text\"));");
        }else if(rp == "type"){
          stk.Push(Gen(((Hashtable)stk.Pop())["type"].ToString(),"text"));
          _c = _c.Append("    _s.Push(Gen(((Hashtable)_s.Pop())[\"type\"].ToString(),\"text\"));");
        }else{
          Error(i,aline,"Unknown command:");
        }
      }else if(sec != ""){
        if(!rb) secl[sec][1] = i;
      }
      /*IL.Emit(OpCodes.Ret);
      tDynamicClass.CreateType();
      tAssembly.SetEntryPoint(tMainMethod);
      tAssembly.Save("a.exe",PortableExecutableKinds.ILOnly,ImageFileMachine.ARM);*/
      //if(File.Exists("out.cs")) File.Delete("out.cs");
      //File.WriteAllText("out.cs","");
      //File.WriteAllText("out.cs",_c + "  }\r\n}");
    }
  }
}