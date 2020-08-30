var fs = require("fs");
var toml = require("toml");
var tmp = toml.parse(fs.readFileSync(".circleci/index.toml") + "");
function fc(i){
  var val = Object.keys(tmp)[i];
  var result = "";
  fs.writeFileSync("main.oba",tmp[val].code);
  var sp = require("child_process").spawn("./obrya",["main.oba"]);
  console.log("-----" + tmp[val].name);
  var f = e=>{
    process.stdout.write(e);
    result+=e + "";
  };
  sp.stdout.on("data",f);
  sp.stderr.on("data",f);
  sp.on("close",exitc=>{
    if(exitc != 0 || result !== tmp[val].output){
      console.log("\r\nError");
      process.exit(1);
    }else{
      if(i + 1 != Object.keys(tmp).length) fc(i + 1);
    }
  });
}
fc(0);
