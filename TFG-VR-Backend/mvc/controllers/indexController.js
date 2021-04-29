const mongoose = require("mongoose");
var bodyParser = require("body-parser");
const { query, request } = require("express");
const User = mongoose.model("User");

const defaultUser = (req, res) => {
  console.log("Connection Succesfull");
  let data = {
    name: "AutoName",
    password: "123",
    current_day: 2,
    dataLevel: {
      total_exp: 0,
      basicOrdersChef: false,
      conditionalIfOrdersChef: false,
      conditionalIfElseOrdersChef: false,
      iterativeOrdersChef: false,
      basicOrdersWaiter: false,
      conditionalIfOrdersWaiter: false,
      conditionalIfElseOrdersWaiter: false,
      iterativeOrdersWaiter: false,
    },
    achievements: {
      firstOrderChef: false,
      twentyOrdersChef: false,
      fortyOrdersChef: false,
      firstIterativeOrderChef: false,
      tenIterativeOrdersChef: false,
      thirtyIterativeOrdersChef: false,
      firstConditionalOrderChef: false,
      tenConditionalOrdersChef: false,
      thirtyConditionalOrdersChef: false,
      firstOrderWaiter: false,
      twentyOrdersWaiter: false,
      fortyOrdersWaiter: false,
      firstIterativeOrderWaiter: false,
      tenIterativeOrdersWaiter: false,
      thirtyIterativeOrdersWaiter: false,
      firstConditionalIfOrderWaiter: false,
      tenConditionalIfOrdersWaiter: false,
      thirtyConditionalIfOrdersWaiter: false,
      firstConditionalIfElseOrderWaiter: false,
      tenConditionalIfElseOrdersWaiter: false,
      thirtyConditionalIfElseOrdersWaiter: false,
    },
    tutorialChefCompleted: false,
    tutorialWaiterCompleted: false,
    volume: "1.0",
    dataCollection: "Start data collection",
  };
  User.create(data, (err, newUser) => {
    if (err) {
      return res.send({ error: err });
    }
    res.send(newUser);
  });
};

const startPage = (req, res) => {
  res.render("index");
};

//Esto nos sirve para coger todos los usuarios
const users = (request, res) => {
  User.find({}).lean().exec((err, data) => { 
    if(err){
        console.log('Error: No se obtuvieron documentos');
        res.send({error : err})

    }else{
      console.log('Success: Se han obtenido todos los usuarios');
        res.send(data);
    }
  });
}

const postUser = ({ body }, res) => {
  var jsonData = body;
  console.log(body);

  Pastel.create(data, (err, newPastel) => {
    if (err) {
      return res.send({ error: err });
    }
    res.send(data);
  });
};
const searchUser = ({ params }, res) => {
  User.findOne({ name:  params.searchName,password:params.password }).lean().exec((err, data) => {
    if (err) {
      return res.send({ error: err });
    }
    res.send(data);
  });
}

const createUser = ({params}, res) => {
  console.log("Connection Succesfull");
  let data = {
    name: params.userName,
    password: params.password,
    dia_actual: 1,
    dataLevel: {
      total_exp: 0,
      allOrdersChef: false,
      basicOrdersChef: false,
      conditionalOrdersChef: false,
      conditionalIfOrdersChef: false,
      conditionalIfElseOrdersChef: false,
      iterativeOrdersChef: false,
      allOrdersWaiter: false,
      basicOrdersWaiter: false,
      conditionalOrdersWaiter: false,
      conditionalIfOrdersWaiter: false,
      conditionalIfElseOrdersWaiter: false,
      iterativeOrdersWaiter: false,
    },
    achievements: {
      firstOrderChef: false,
      tenOrdersChef: false,
      twentyfiveOrdersChef: false,
      fiftyOrdersChef: false,
      seventyfiveOrdersChef: false,
      hundredOrdersChef: false,
      firstBasicOrderChef: false,
      tenBasicOrdersChef: false,
      twentyfiveBasicOrdersChef: false,
      fourtyBasicOrdersChef: false,
      firstConditionalOrderChef: false,
      tenConditionalOrdersChef: false,
      thirtyConditionalOrdersChef: false,
      fiftyConditionalOrdersChef: false,
      firstConditionalIfOrderChef: false,
      tenConditionalIfOrdersChef: false,
      thirtyConditionalIfOrdersChef: false,
      firstConditionalIfElseOrderChef: false,
      tenConditionalIfElseOrdersChef: false,
      thirtyConditionalIfElseOrdersChef: false,
      firstIterativeOrderChef: false,
      tenIterativeOrdersChef: false,
      thirtyIterativeOrdersChef: false,
      firstOrderWaiter: false,
      tenOrdersWaiter: false,
      twentyfiveOrdersWaiter: false,
      fiftyOrdersWaiter: false,
      seventyfiveOrdersWaiter: false,
      hundredOrdersWaiter: false,
      firstBasicOrderWaiter: false,
      tenBasicOrderWaiter: false,
      twentyfiveBasicOrdersWaiter: false,
      fourtyBasicOrdersWaiter: false,
      firstConditionalOrderWaiter: false,
      tenConditionalOrdersWaiter: false,
      thirtyConditionalOrdersWaiter: false,
      fiftyConditionalOrdersWaiter: false,
      firstConditionalIfOrderWaiter: false,
      tenConditionalIfOrdersWaiter: false,
      thirtyConditionalIfOrdersWaiter: false,
      firstConditionalIfElseOrderWaiter: false,
      tenConditionalIfElseOrdersWaiter: false,
      thirtyConditionalIfElseOrdersWaiter: false,
      firstIterativeOrderWaiter: false,
      tenIterativeOrdersWaiter: false,
      thirtyIterativeOrdersWaiter: false,
    },
    tutorialChefCompleted: false,
    tutorialWaiterCompleted: false,
    volume: "0.5",
    dataCollection: "Start data collection",
  };
  User.create(data, (err, newUser) => {
    if (err) {
      return res.send({ error: err });
    }
    res.send(newUser);
  });
};


const updateUser = ({body},res)=>{
  console.log ("AQUI EMPIEZA EL BODY");
  console.log(body.name);
  User.updateOne({name: body.name,password:body.password},body,(err,result)=>{
    if(err){
      res.send(err);
    }else{
      res.json(result);
    }
  })
};


module.exports = { defaultUser, postUser, startPage,searchUser,createUser,updateUser,users};