
var express = require("express");
var ctrIndex = require("../controllers/indexController");
var router = express.Router();

/* GET home page. */
router.get("/", ctrIndex.startPage);
router.get("/default", ctrIndex.defaultUser);
router.post("/postUser",ctrIndex.postUser);
router.get("/searchUser/:searchName&:password",ctrIndex.searchUser);
router.get("/users", ctrIndex.users);
router.get("/createUser/:userName&:password",ctrIndex.createUser);
router.put("/sendUser",ctrIndex.updateUser);

module.exports = router;