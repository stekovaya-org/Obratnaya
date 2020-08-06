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
  public static string Append(this string str,string srt){
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
  public static bool Check(string str,Type type){
    bool r = true;
    try{
      type.GetMethod("Parse",new Type[]{typeof(string)}).Invoke(null,new object[]{str});
    }catch{
      r = false;
    }
    return r;
  }
  public static Hashtable Gen(string data,string type){
    return new Hashtable{
      ["data"] = data,
      ["type"] = type
    };
  }
  public static void Error(int i,string line,string content){
    Console.Error.WriteLine("\x1b[1m:" + (i + 1) + ": \x1b[m\x1b[31m" + content + "\x1b[m\r\n" + line);
    Environment.Exit(1);
  }
  public static void Error(string content){
    Console.Error.WriteLine("\x1b[1m:FATAL: \x1b[m\x1b[31m" + content + "\x1b[m");
    Environment.Exit(1);
  }
  public static void Run(string code){
    var dstk = new System.Collections.Generic.List<Hashtable>();
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
    var _d = new System.Collections.Generic.List<Hashtable>();
    var _s = new Stack();
";
    var print = typeof(Console).GetMethod("WriteLine", new Type[]{ typeof(string) });
    if(Regex.Matches(code,@"/\*").Count != Regex.Matches(code,@"\*/").Count) Error("Unmatched comment bracket:");
    for(int i = 0; i < all.Length; i++){
      string aline = all[i];
      string[] line = aline.Split(" ");
      if(sec == "data" && line[0] != ".main:"){
        string rp = Regex.Replace(aline,"^(?:\t\t|    )([^\n]+)$","$1");
        if(rp.StartsWith(".text ")){
          string rpl = Regex.Replace(Regex.Replace(rp,@"^\.text ([^\n]+)$","$1"),@"\$<([0123456789ABCDEFabcdef]+)>",m=>(Convert.ToChar(Convert.ToInt32(m.Groups[1].ToString(),16)).ToString()).ToString());
          IL.Emit(OpCodes.Ldstr,rpl);
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem,typeof(string));
          dstk.Add(Gen(rpl,"text"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl.Replace("\"","\\\"") + "\",\"text\"));");
        }else if(rp.StartsWith(".decimal ")){
          string rpl = Regex.Replace(rp,@"^\.decimal ([^\n]+)$","$1");
          if(!Check(rpl,typeof(decimal))) Error(i,aline,"Cannot convert text to decimal:");
          IL.Emit(OpCodes.Ldc_R4,Single.Parse(rpl));
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem_R4);
          dstk.Add(Gen(rpl,"decimal"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl + "\",\"decimal\"));");
        }else if(rp.StartsWith(".boolean ")){
          string rpl = Regex.Replace(rp,@"^\.boolean ([^\n]+)$","$1");
          if(rpl != "0" && rpl != "1") Error(i,aline,"Cannot convert to boolean:");
          IL.Emit(OpCodes.Ldc_I4,int.Parse(rpl));
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem_I4);
          dstk.Add(Gen(rpl,"boolean"));
          _c = _c.Append("    _d.Add(Gen(\"" + rpl + "\",\"boolean\"));");
        }else if(rp == ".empty"){
          IL.Emit(OpCodes.Ldstr,"");
          IL.Emit(OpCodes.Ldc_I4,dstk.Count);
          IL.Emit(OpCodes.Stelem,typeof(string));
          dstk.Add(Gen("","text"));
          _c = _c.Append("    _d.Add(Gen(\"\",\"text\"));");
        }else{
          Error(i,aline,"Unknown prefix:");
        }
      }
      if(sec == "main"){
        string rp = Regex.Replace(aline,"^(?:\t\t|    )([^\n]+)$","$1");
        if(rp.StartsWith("/*")) empo = true;
        if(empo && !rp.EndsWith("*/")) continue;
        if(rp.EndsWith("*/")){
          empo = false;
          continue;
        }
        if(rp.StartsWith(";")){
          continue;
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
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
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
            _c = _c.Append("    _da[1] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
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
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot subtract not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] - arr[1]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[0] - _da[1]).ToString(),\"decimal\"));");
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
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
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
            _c = _c.Append("    _da[1] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
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
          if(cp.Length != 2) Error(i,aline,"Arguments must be 2:");
          decimal[] arr = {0,0};
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot divide not decimal(s):");
            arr[0] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[0],typeof(decimal))){
            arr[0] = decimal.Parse(cp[0]);
            _c = _c.Append("    _da[0] = " + cp[0] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          if(cp[1] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot divide not decimal(s):");
            arr[1] = decimal.Parse(s["data"].ToString());
            _c = _c.Append("    _da[1] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] / arr[1]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[0] / _da[1]).ToString(),\"decimal\"));");
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
            _c = _c.Append("    _da[0] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
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
            _c = _c.Append("    _da[1] = (decimal)((Hashtable)_s.Pop())[\"data\"];");
          }else if(Check(cp[1],typeof(decimal))){
            arr[1] = decimal.Parse(cp[1]);
            _c = _c.Append("    _da[1] = " + cp[1] + ";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(Gen((arr[0] % arr[1]).ToString(),"decimal"));
          _c = _c.Append("    _s.Push(Gen((_da[0] % _da[1]).ToString(),\"decimal\"));");
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
          if(File.Exists("out.cs")) File.Delete("out.cs");
          File.WriteAllText("out.cs","");
          File.WriteAllText("out.cs",_c + "  }\r\n}");
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
          if(File.Exists("out.cs")) File.Delete("out.cs");
          File.WriteAllText("out.cs","");
          File.WriteAllText("out.cs",_c + "  }\r\n}");
          Environment.Exit(intv);
        }else if(rp.StartsWith("jmp ")){
          string[] cp = rp.Ccut("jmp");
          if(cp.Length != 1) Error(i,aline,"Arguments must be 2:");
          Hashtable s;
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            s = (Hashtable)stk.Pop();
            if(s["type"].ToString() != "decimal") Error(i,aline,"Cannot jump by not positive integer:");
            if(!Check(cp[0],typeof(uint))) Error(i,aline,"Cannot jump by not positive integer:");
            i = int.Parse(s["data"].ToString()) - 2;
          }else if(Check(cp[0],typeof(uint))){
            i = int.Parse(cp[0]) - 2;
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
          stk.Push(Gen(arr[1] + arr[0],"text"));
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
          if(cp.Length != 1) Error(i,aline,"Arguments must be 1:");
          Hashtable sd = Gen("","");
          if(cp[0] == "@"){
            stk.Underflow(1,i,aline);
            sd = (Hashtable)stk.Pop();
            sd["type"] = "undefined";
            sd["data"] = sd["data"].ToString();
            _c = _c.Append("    _sa[0] = (string)(((Hashtable)_s.Pop())[\"data\"].ToString());");
          }else if(Check(cp[0],typeof(decimal))){
            sd = Gen(cp[0],"undefined");
            _c = _c.Append("    _sa[0] = \"" + cp[0] + "\";");
          }else{
            Error(i,aline,"Unknown format:");
          }
          stk.Push(sd);
          _c = _c.Append("    _s.Push(Gen(_sa[0],\"undefined\"));");
        }else if(rp == "type"){
          stk.Push(Gen(((Hashtable)stk.Pop())["type"].ToString(),"text"));
          _c = _c.Append("    _s.Push(Gen(((Hashtable)_s.Pop())[\"type\"].ToString(),\"text\"));");
        }else{
          Error(i,aline,"Unknown command:");
        }
      }
      if(line[0] == ".main:"){
        sec = "main";
      }
      if(line[0] == ".data:"){
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
      }
      /*IL.Emit(OpCodes.Ret);
      tDynamicClass.CreateType();
      tAssembly.SetEntryPoint(tMainMethod);
      tAssembly.Save("a.exe",PortableExecutableKinds.ILOnly,ImageFileMachine.ARM);*/
      if(File.Exists("out.cs")) File.Delete("out.cs");
      File.WriteAllText("out.cs","");
      File.WriteAllText("out.cs",_c + "  }\r\n}");
    }
  }
}