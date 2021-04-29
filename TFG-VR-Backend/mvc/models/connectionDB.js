const mongoose = require("mongoose");

//let uri = "mongodb://localhost/TFG2020";


//Aqui hemos cambiado MONGODB_URI en Heroku . 
if (process.env.NODE_ENV === 'production'){
   //uri = process.env.MONGODB_URI;
   //Esto hay que conseguir que se pueda utilizar con Heroku
   uri="mongodb+srv://alexfuentesraventos:Baloncesto8@cluster-qbp6st97.lg4ch.mongodb.net/heroku_qbp6st97?retryWrites=true&w=majority";

}

mongoose.connect(uri, { useUnifiedTopology: true, useNewUrlParser: true });

mongoose.connection.on("connected", () => {
  console.log("=============== ");
  console.log(`MongoDB connected to ${uri}`);
  console.log("================== ");
});

const shutdown = (msg, callback) => {
  mongoose.connection.close(() => {
    console.log(`Mongoose disconnected through ${msg}`);
    callback();
  });
};

process.once("SIGUSR2", () => {
  shutdown("nodemon restart", () => {
    process.kill(process.pid, "SIGUSR2");
  });
});

process.on("SIGINT", () => {
  shutdown("app termination", () => {
    process.exit(0);
  });
});

process.on("SIGTERM", () => {
  shutdown("Heroku app shutdown", () => {
    process.exit(0);
  });
});

require("./users.js");
