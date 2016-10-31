(function() {
    var levels = JSON.stringify([{id:0,name:'Baixo'},{id:1,name:'Padrão'},{id:2,name:'Moderado'},{id:3,name:'Urgente'}]);
    $('#financeType').find("select").simpleSelect2Json("http://localhost:5000/api/finances/financeTypes", 'id', 'name');
    $('#place').find("select").simpleSelect2Json("http://localhost:5000/api/finances/places", 'id', 'name');
    $('#level').simpleSelect2Json(levels, 'id', 'name');
})();

$(document).ready(function() {
    var button;
    $("#createFinance").on("click", function() {
        var fType   = {id: $("#financeType").find("select").val(), name: ""},
            place   = {id: $("#place").find("select").val(), name: ""},
            value   = $("#value").find("input").val(),
            date    = $("#date").find("input").val(),
            level   = $("#level").val(),
            status  = (Number ($("#status")[0].checked)),
            userId = $("#userId").val();
            
        button = $(this);

        if (validateFields(fType,place,date,value)) {
            button.button('loading');
            var url = "Create",
                contentType = 'application/json; charset=utf-8',
                data = JSON.stringify({
                    financeType: fType, 
                    place: place, 
                    value: value, 
                    date: date, 
                    level: level,
                    status: status,
                    userId: userId});
            console.debug("POST ", url, ": ", data);
            $.ajax({
                url: url,
                type: "POST",
                contentType: contentType,
                data: data,
                success: function(data) {
                    button.button('reset');
                    console.debug(data);
                    if (data.code == 0) {
                        $('.toast').text("Finança salva!").fadeIn(400).delay(2500).fadeOut(400);
                        $("#financeType").find("select").val("0").trigger("change");
                        $("#place").find("select").val("0").trigger("change");
                        $("#value").find("input").val("");
                        $("#date").find("input").val("");
                        $("#level").val("0").trigger("change");
                        $("#status")[0].checked = false;
                    }
                    else {
                        $('.toast').text(data.message).fadeIn(400).delay(2500).fadeOut(400);
                    }
                },
                error: function(err) {
                    button.button('reset');
                    alert("Problema! Verifique sua conexão com a internet e tente novamente.");
                    console.log(err);
                }
            });
        }
    });

    function validateFields(fType, place, date, value) {
        if (!fType.id) {
            $("#financeType").find("span[class='text-danger']").text("Invalid!");
            $("#financeType").parent().attr({class: "form-group row has-error"});
        }
        if (!place.id) {
            $("#place").find("span[class='text-danger']").text("Invalid!");
            $("#place").parent().attr({class: "form-group row has-error"});
        }
        if (!date) {
            $("#date").find("span").text("Invalid!");   
            $("#date").parent().attr({class: "form-group row has-error"}); 
           
        }
        if (!value) {
            $("#value").find("span").text("Invalid!");
            $("#value").parent().attr({class: "form-group row has-error"});         
        }
        return ioButton(fType,place,date,value);
    }

    function ioButton(fType, place, date, value) {
        if (fType.id && place.id && date && value) {
            if (button) button.attr({disabled: false});
            return true;
        } 
        if (button) button.attr({disabled: true});
    }

    $("#date").find("input").on("change", function() {
        $("#date").find("span").text("");
        $("#date").parent().attr({class: "form-group row"});
        if ($("#value").find("input").val()) {
             if (button) button.attr({disabled: false});
        }
    });
    $("#value").find("input").on("keyup", function() {
        $("#value").find("span").text("");
        $("#value").parent().attr({class: "form-group row"});
        if ($("#date").find("input").val()) {
             if (button) button.attr({disabled: false});
        }
    });

    $("#financeType").find("select").on("change", function() {
        $("#financeType").find("span[class='text-danger']").text("");
        $("#financeType").parent().attr({class: "form-group row"});
        if ($("#place").find("select").val()) {
             if (button) button.attr({disabled: false});
        }
    });

    $("#place").find("select").on("change", function() {
        $("#place").find("span[class='text-danger']").text("");
        $("#place").parent().attr({class: "form-group row"});
        if ($("#financeType").find("select").val()) {
             if (button) button.attr({disabled: false});
        }
    });
    
});