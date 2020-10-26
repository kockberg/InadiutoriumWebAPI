var endDateInput = document.getElementById("endDate");
var startDateInput = document.getElementById("startDate");
var searchBtn = document.getElementById("searchBtn");

var inputs = [endDateInput, startDateInput];

for (var i = 0; i < inputs.length; i++) {
    inputs[i].addEventListener("keyup", function() { validate() }, true);
}

var alertArea = document.getElementById("alertArea");
alertArea.style.display = "none";
searchBtn.disabled = true;

function validate() {
    var startDate = new Date(startDateInput.value).getTime();
    var endDate = new Date(endDateInput.value).getTime();

    // Dates are parsed. Start checking.
    if (isNaN(startDate) || isNaN(endDate)) {
        //console.log("Values are not ready");
        return;
    }

    // Check that the given dates are over the minimum value.
    var minimumDate = new Date("01.01.1970").getTime();

    if (startDate < minimumDate || endDate < minimumDate) {
        //console.log("Values are under minimum Date");
        return;
    }

    // Values are ready and over the minimum value.
    if (startDate > endDate) {
        console.log("startDate is greater than endDate\nSearch disabled.")
        alertArea.style.display = "block";
        document.getElementById("alertTextField").innerHTML = "Start date can't be greater than end date";
        searchBtn.disabled = true;
        return;
    }

    // Values are ok.
    //console.log("Values are ok to search");
    alertArea.style.display = "none";
    searchBtn.disabled = false;
}