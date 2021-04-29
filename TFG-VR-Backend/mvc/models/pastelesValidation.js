const isValid = function (nombrePastel, array_fecha) {
  let val_numb = array_fecha.length === 3;
  let val_format = isValidFormat(array_fecha);
  let isValidName = nombrePastel.length > 0;

  return val_numb && val_format && isValidName;
};
const isValidFormat = function (array_fecha) {
  return array_fecha.every(validationFunction);
};

const validationFunction = (currentValue, index) => {
  return index == 2
    ? currentValue <= new Date().getFullYear()
    : currentValue.length >= 2 &&
        (index == 0 ? currentValue < 32 : currentValue < 13) &&
        currentValue > 0;
};

module.exports = {
  isValid: isValid,
};
