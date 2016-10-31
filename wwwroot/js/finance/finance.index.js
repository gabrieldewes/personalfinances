$(document).ready(function() {
    "use strict";
    var modelId; 
    $('#delete_modal').on('show.bs.modal', function (event) {
        modelId = $(event.relatedTarget).data('model-id');
    });

    $("#delete_modal").ready(function() {    
        $("#confirm_delete").on("click", function() { 
            var $button = $(this);
            $button.button('loading');
            
            var contentType = 'application/json; charset=utf-8',
                data        = JSON.stringify(modelId),
                url         = "/Finances/DeleteFinance";
            console.debug("POST "+ url + ": "+ data);
            $.ajax({
                url: url,
                type: "POST",
                contentType: contentType,
                data: data,
                success: function(data) {
                    console.debug(data);
                    if (data.code == 0) {
                        $('.toast').text(data.message).fadeIn(400).delay(3000).fadeOut(400);
                        $('body').find('table').find("tr[data-model-id='"+modelId+"']").remove();
                        $("#delete_modal").modal('hide');
                    }
                    else {
                        $("#delete_modal").modal('hide');
                        $('.toast').text(data.message).fadeIn(400).delay(3000).fadeOut(400);
                    }
                    $button.button('reset');
                },
                error: function(err) {
                    $button.button('reset');
                    console.debug(err);
                    alert("Problema! Verifique sua conexão com a internet e tente novamente.");
                }
            });
        });
    });

    $("[data-toggle='popover']").popover({
        html: true,
        title: '<span class="text-danger"><strong>Atenção meu querido</strong></span>'+
                '<button type="button" class="btn btn-default close"\ onclick="$(&quot;a#update-status-popover&quot;).popover(&quot;hide&quot;);">×</button>',
        placement: 'right',
        content: function() {
            var btnCls  = $(this).prop("class"),
                btnTxt  = $(this).text(),
                modelId = $(this).data('model-id'),
                icon,
                modelStatus;
            if (btnTxt == "Pago") {
                btnTxt = " Pendente";
                btnCls = "btn btn-warning";
                icon = "glyphicon glyphicon-remove";
                modelStatus = 0;
            }
            else if (btnTxt == "Pendente") {
                btnTxt = " Pago";
                btnCls = "btn btn-success";
                icon = "glyphicon glyphicon-ok";
                modelStatus = 1;
            }
            return "Atualizar Status de Finança <br/><br/><button class='"+btnCls+"' data-status='"+modelStatus+"' data-model-id='"+modelId+"' id='confirm_status'><span class='"+icon+"'></span>"+btnTxt+"</button>";
        }
    })
    .parent().delegate('button#confirm_status', 'click', function() {
        console.log("Model id ", $(this).data('model-id'), "status ", $(this).data('status'));
        var modelId     = $(this).data('model-id'),
            modelStatus = $(this).data('status'),
            btnCls      = modelStatus == 1 ? "label label-success":"label label-warning",
            btnTxt      = modelStatus == 1 ? "Pago":"Pendente",
            url = "Finances/UpdateStatus",
            data = JSON.stringify({id: modelId, status: modelStatus});
        console.debug("POST ", url, ": ", data);
        $.ajax({
            url: url,
            type: "POST",
            contentType: 'application/json; charset=UTF-8',
            data: data,
            success: function(data) {
                console.debug(data);
                $('.toast').text(data.message).fadeIn(400).delay(2500).fadeOut(400);
                $("#finance-table").find("tr[data-model-id='"+modelId+"'] td a#update-status-popover").attr({class: btnCls}).text(btnTxt);
                $('a#update-status-popover').popover('hide');
            },
            error: function(err) {
                console.error(err);
                $('a#update-status-popover').popover('hide');
            }
        });
    });
});