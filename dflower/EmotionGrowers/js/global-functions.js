// 2024-01-08 AleG: Global function that converts today date to format yyyy-mm-dd, suitable for input type=date
function formatTodayForInputs() {
    return Date.today().toString("yyyy-MM-dd");
}


jQuery.fn.valAsDate = function () {
    // `this` is the jQuery Object on which the yourFunctionName method is called.
    // `arguments` will contain any arguments passed to the yourFunctionName method.
    var firstElement = this[0];
    firstElement.valueAsDate = arguments[0];
    return this; // Needed for other methods to be able to chain off of yourFunctionName.
};