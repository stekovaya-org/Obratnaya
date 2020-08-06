var fs = require("fs");
var toml = require("toml");
var tmp = toml.parse(fs.readFileSync(".circleci/index.toml") + "");
for(var val in tmp){
  fs.writeFileSync("main.oba",val.code);
  var sp = require("child_process").spawn("mono",["obrya.exe","main.oba"]);
  console.log("-----" + val.name);
  sp.stdout.on("data",e=>{
    process.stdout.write(e);
  });
  sp.on("close",exitc=>{
    if(exitc != 0){
      console.log("\r\nError");
      process.exit(1);
    }
  });
}
