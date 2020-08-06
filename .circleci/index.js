var fs = require("fs");
var toml = require("toml");
var tmp = toml.parse(fs.readFileSync(".circleci/index.toml") + "");
for(var val in tmp){
  var result = "";
  fs.writeFileSync("main.oba",tmp[val].code);
  var sp = require("child_process").spawn("mono",["obrya.exe","main.oba"]);
  console.log("-----" + tmp[val].name);
  sp.stdout.on("data",e=>{
    process.stdout.write(e);
    result+=e + "";
  });
  sp.on("close",exitc=>{
    if(exitc != 0 || result !== tmp[val].output){
      console.log("\r\nError");
      process.exit(1);
    }
  });
}
