
function CreateDataTable(id, oSorting, oPaginate, oServerSide, aoColumnDefs, oInfo) {
    var param = {};

    param.oLanguage = {
        "sUrl": GetPath("/Content/dataTables_lang.txt")
    };
    param.bLengthChange = true;
    param.bInfo = oInfo == null ? true : oInfo;
    param.aLengthMenu = [10, 20, 30, 40, 50, 60, 70, 80, 90, 100];
    param.bStateSave = oPaginate.bStateSave;
    param.stateDuration = -1;
    param.sDom = '<"tableHead"ipl>t';

    param.fnStateSaveCallback = function (oSettings, oData) {
        $.ajax({
            "url": "/Common/SaveDataTableState",
            "data": { path: window.location.pathname, data: JSON.stringify(oData) },
            "dataType": "json",
            "method": "POST",
            "success": function() {
            }
        });
    };

    param.fnStateLoadCallback = function (oSettings) {
        var o;

        // Send an Ajax request to the server to get the data. Note that
        // this is a synchronous request.
        $.ajax({
            "url": "/Common/GetDataTableState",
            "data": { path : window.location.pathname },
            "async": false,
            "dataType": "json",
            "success": function (json) {
                o = json;
            }
        });

        if( o != null)
            return JSON.parse(o);
        else 
            return null;
    };

    param.bProcessing = false;
    param.bFilter = false;

    if (oSorting != null) {
        param.bSort = oSorting.bSort;
        if (oSorting.bSort && oSorting.aaSorting != null) {
            param.aaSorting = oSorting.aaSorting;
        }
    }

    if (oPaginate != null && oPaginate.bPaginate != null) {
        param.bPaginate = oPaginate.bPaginate;
        if (oPaginate.bPaginate) {
            param.sPaginationType = "full_numbers";
            if (oPaginate.iDisplayLength) {
                param.iDisplayLength = oPaginate.iDisplayLength;
            }
        }
    }

    
    if (oServerSide != null && oServerSide.bServerSide != null) {
        param.bServerSide = oServerSide.bServerSide;
        if (oServerSide.bServerSide) {
            param.sAjaxSource = oServerSide.sAjaxSource;

            if (oServerSide.fnServerParams != null) {
                param.fnServerParams = oServerSide.fnServerParams;
            }

            param.fnServerData = function (sSource, aoData, fnCallback) {
                if (typeof (sort_colum) != 'undefined' && typeof (sort_type) != 'undefined') {
                    for (var i = 0; i < aoData.length; i++) {
                        if (aoData[i].name == 'iSortCol_0') {
                            aoData[i].value = sort_colum;
                            aoData[i + 1].value = sort_type;
                            break;
                        }
                    }
                }

                var ajaxParam = {};
                ajaxParam.dataType = 'json';
                ajaxParam.type = "POST";
                ajaxParam.async = true;
                ajaxParam.url = sSource;
                ajaxParam.data = aoData;

                if (oServerSide.fnBeforeSend) {
                    ajaxParam.beforeSend = oServerSide.fnBeforeSend;
                }

                if (oServerSide.fnDrawCallback) {
                    ajaxParam.complete = oServerSide.fnDrawCallback;
                }

                ajaxParam.success = function (data, status, xhr) {
                    if (data.ErrorMessages == null) {
                        if (oServerSide.fnInitComplete) {
                            oServerSide.fnInitComplete(data);
                        }
                        fnCallback(data, status, xhr);
                    }
                    else {
                        alert(data.ErrorMessages);
                    }
                }
                ajaxParam.error = function (err) {
                    window.location.href = '/Error';
                }
                $.ajax(ajaxParam);
            }
        }
    }

    if (aoColumnDefs != null) {
        param.aoColumnDefs = aoColumnDefs;
    }

    var dataTable = $(id).dataTable(param);
    return dataTable;
}
