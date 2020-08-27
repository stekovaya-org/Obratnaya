using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using static Data;
using static Obratnaya;

class MainClass {
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
      while(true){
        Console.Write(">>> ");
        string q = Console.ReadLine();
        if(q == ".bye"){
          Environment.Exit(0);
        }else if(q == ".license"){
          Console.WriteLine(Data.License.LegalCode);
        }else{
          Console.WriteLine("REPL version isnt available now");
          //Obratnaya.Run(q);
        }
      }
    }else{
      if(args[0] == "--version"){
        Console.WriteLine(Data.Version.FullVersion);
        Environment.Exit(0);
      }
      if(args[0] == "--license"){
        Console.WriteLine(Data.License.LegalCode);
        Environment.Exit(0);
      }
      if(File.Exists(args[0])){
        Obratnaya.Run(File.ReadAllText(args[0]));
      }else{
        Console.Error.WriteLine("\x1b[1mObratnaya: \x1b[31merror: \x1b[m" + args[0] + ": No such file");
        Console.Error.WriteLine("\x1b[1mObratnaya: \x1b[31mfatal error: \x1b[mno input files");
        Environment.Exit(1);
      }
    }
  }
}