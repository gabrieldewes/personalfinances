$(document).ready(function() {
    (function() {
        "use strict";
        var dataType,
            content,
            url,
            tableId,
            userId = $("#userId").val();
        $('#modal').on('show.bs.modal', function (event) {
            var button   = $(event.relatedTarget),
                modal    = $(this),
                dataType = button.data('type'),
                name     = button.data('name');
            modal.find('.modal-title').text('Novo ' + name);
            if (dataType == "FinanceType") {
            url = "/Finances/CreateFinanceType";
            tableId = "finance_type_table";
            }
            else if (dataType == "Place") {
            url = "/Finances/CreatePlace";
            tableId = "place_table";
            } 
        });
        
        $('#modal_confirm').on("click", function() {
            var $button = $(this);
            $button.button('reset');
            
            content = $("#modal_content").val();
            var contentType = 'application/json; charset=utf-8';
            var data = JSON.stringify({id: userId, name: content});
            console.debug("POST ", url, ": ", data);
            $.ajax({
                url: url,
                type: "POST",
                contentType: contentType,
                data: data,
                success: function(data) {
                    console.debug(data);
                    $button.button('reset');
                    $("#modal_content").val("");
                    if (data.type != "error") {
                        $("#"+tableId).find("tbody").prepend(
                            "<tr data-model-id='"+data.code+"'><td>"+data.code+"</td><td class='text-info'><strong>"+data.message+"</strong></td>" +
                            "<td><a data-model-id='"+data.code+"' data-toggle='modal' data-target='#delete_modal' data-name='"+data.message+"' data-type='"+data.type+"' href='#'><span class='glyphicon glyphicon-trash text-danger'></span></a></td></tr>"
                        );
                        $('.toast').text(data.message +" Salvo!").fadeIn(400).delay(2500).fadeOut(400);
                        $("#modal").modal('hide');
                    }
                },
                error: function(err) {
                    $button.button('reset');
                    console.debug(err);
                    alert("Problema! Verifique sua conexão com a internet e tente novamente.");
                }

            });
        });
    })();

    (function() {
        "use strict";
        var modelId,
            tableId,
            modelName,
            financeUrl,
            dataType;

        $('#delete_modal').on('show.bs.modal', function (event) {
            var button = $(event.relatedTarget),
                modal  = $(this);
            modelName = button.data('name'); 
            dataType  = button.data('type');
            modelId   = button.data('model-id');
            modal.find('.modal-body p').text('Tem certeza que deseja apagar '+ modelName +'?');
            if (dataType == "FinanceType") {
            financeUrl = "/Finances/DeleteFinanceType";
            tableId = "finance_type_table";
            }
            else if (dataType == "Place") {
            financeUrl = "/Finances/DeletePlace";
            tableId = "place_table";
            }
        });

        $("#confirm_delete").on("click", function() { 
            var $button = $(this);
            $button.button('loading');
            
            var $table = $("#"+tableId),
                contentType = 'application/json; charset=utf-8',
                data = JSON.stringify(modelId);
            console.debug("POST "+ financeUrl + ": "+ data);
            $.ajax({
                url: financeUrl,
                type: "POST",
                contentType: contentType,
                data: data,
                success: function(data) {
                    if (data.code == 0) {
                        console.debug(data);
                        $('.toast').text(modelName +" Apagado!").fadeIn(400).delay(3000).fadeOut(400);
                        $table.find("tr[data-model-id='"+modelId+"']").remove();
                        $button.button('reset');
                        $("#delete_modal").modal('hide');
                    }
                    else {
                        $button.attr({disabled: false});
                        $("#delete_modal").modal('hide');
                        $('.toast').text(data.message).fadeIn(400).delay(3000).fadeOut(400);
                    }
                },
                error: function(err) {
                    $button.attr({disabled: false});
                    console.debug(err);
                    alert("Problema! Verifique sua conexão com a internet e tente novamente.");
                }
            });
        });
    })();
});
