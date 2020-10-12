using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using static Data;
using static Obratnaya;

namespace obrya {
class MainClass {
  public static System.Collections.Generic.Dictionary<string,string> NewProgram(string program, string description){
    return new System.Collections.Generic.Dictionary<string,string>{
      ["program"] = program,
      ["description"] = description
    };
  }
  public static void Center(string ascii){
    var spcs = new string[(int)Math.Abs(Math.Floor((decimal)((Console.WindowWidth - ascii.Replace("\r","").Split(new char[]{'\n'}).Select(x=>x.Length).ToArray().Max()) / 2)))];
    Console.WriteLine(string.Join("\r\n",ascii.Replace("\r","").Split(new char[]{'\n'}).Select(x=>string.Join(" ",spcs) + x).ToArray()));
  }
  public static void Center(string ascii,int spaces){
    string spcs = "";
    for(int cs = 0; cs < spaces; cs++) spcs+=" ";
    Center(string.Join(spcs,ascii.ToCharArray()));
  }
  public static void Main (string[] args) {
    if(args.Length == 0){
      string fs = "";
      string pha = "";
      var programs = new System.Collections.Generic.Dictionary<string,System.Collections.Generic.Dictionary<string,string>>{
        ["hello-world"] = NewProgram(@"#include <stdout.oba>
.main:
    push Hello, world!
    push msgln
    call @","Display \"Hello, world!\"")
      };
      Center(@"   ____  __               __                         
  / __ \/ /_  _________ _/ /_____  ____ ___  ______ _
 / / / / __ \/ ___/ __ `/ __/ __ \/ __ `/ / / / __ `/
/ /_/ / /_/ / /  / /_/ / /_/ / / / /_/ / /_/ / /_/ / 
\____/_.___/_/   \__,_/\__/_/ /_/\__,_/\__, /\__,_/  
                                      /____/         ");
      Center("Welcome to Obratnaya version " + Data.Version.FullVersion);
      Center("Copyright (C) 2020  TheForArkLD, NoNameByProgram, RaidTheWeb,");
      Center("SixBeeps");
      Center("Type .bye to exit");
      Center("Type .license to display license");
      Center("Type .run to run");
      Center("Type \".fs n\" to set first spaces");
      Center("Type .list to display example programs");
      while(true){
        Console.Write(">>>" + fs);
        string q = Console.ReadLine();
        if(q == ".bye"){
          Environment.Exit(0);
        }else if(q == ".license"){
          Console.WriteLine(Data.License.LegalCode);
        }else if(q == ".run"){
          Obratnaya.Run(pha);
          if(erd) Console.WriteLine(erc);
          pha = "";
          fs = "";
        }else if(q == ".list"){
          int cfd = 1;
          Console.WriteLine("Select program by number:");
          foreach(var d in programs){
            Console.WriteLine(cfd.ToString() + ") " + d.Key + ": " + d.Value["description"]);
            cfd++;
          }
          Console.WriteLine(cfd.ToString() + ") Cancel");
          typeagain:
          Console.Write(">");
          string psg = Console.ReadLine();
          string[] par = new string[programs.Count + 1].Select((element,index)=>{
            return (index + 1).ToString();
          }).ToArray();
          if(par.Contains(psg)){
            int pit = int.Parse(psg);
            if(pit == cfd) continue;
            Console.WriteLine(string.Join("-",new string[Console.WindowWidth]));
            string prg = programs.Values.ToArray()[pit - 1]["program"];
            Console.WriteLine(prg);
            Console.WriteLine(string.Join("-",new string[Console.WindowWidth]));
            Obratnaya.Run(prg);
            if(erd) Console.WriteLine(erc);
            Console.WriteLine(string.Join("-",new string[Console.WindowWidth]));
          }else{
            Console.WriteLine("Unknown option: " + psg);
            goto typeagain;
          }
        }else if(q.StartsWith(".fs ")){
          string fss = Regex.Replace(q,@"^\.fs ([^\r\n]+)$","$1");
          if(!(Obratnaya.Check(fss,typeof(uint)) && Obratnaya.Check(fss,typeof(int)))){
            Console.Error.WriteLine("Not a positive number: .fs \x1b[1m\x1b[31m" + fss + "\x1b[m");
            continue;
          }
          fs = (fss == "0" ? "" : " ") + string.Join(" ",new string[int.Parse(fss)]);
        }else{
          //Console.WriteLine("REPL version isnt available now");
          //Obratnaya.Run(q);
          //Console.WriteLine("Ready.");
          pha += fs + q + "\r\n";
        }
      }
    }else{
      if(args[0] == "--version" || args[0] == "--v"){
        Console.WriteLine(Data.Version.FullVersion);
        Environment.Exit(0);
      }
      if(args[0] == "--license" || args[0] == "--l"){
        Console.WriteLine(Data.License.LegalCode);
        Environment.Exit(0);
      }
      if(File.Exists(args[args.Length - 1])){
        string[] vargs = args;
        Array.Resize(ref vargs,vargs.Length - 1);
        if(!vargs.All(x=>x == "--info" || x == "--i" || x == "--log" || x == "-l")){
          Console.Error.WriteLine("Unknown option");
          Environment.Exit(1);
        }
        oti = args.Contains("--i") || args.Contains("--info");
        adv = args.Contains("-l") || args.Contains("--log");
        Obratnaya.Run(File.ReadAllText(args[args.Length - 1]),oti);
        Environment.Exit(erd ? 1 : 0);
      }else{
        Console.Error.WriteLine("\x1b[1mObratnaya: \x1b[31merror: \x1b[m" + args[args.Length - 1] + ": No such file");
        Console.Error.WriteLine("\x1b[1mObratnaya: \x1b[31mfatal error: \x1b[mno input files");
        Environment.Exit(1);
      }
    }
  }
}
}