/**********************************************************************
 * BookList handles the book list, especially the 'filter by' part
 * 
 * First created: 2016/09/22
 * 
 * Under the MIT License (MIT)
 *
 * Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
 **********************************************************************/

var BookList = (function($, loggingDisplay) {
    'use strict';

    var filterApiUrl = null;

    function enableDisableFilterDropdown($fsearch, enable) {
        var $fvGroup = $('#filter-value-group');
        if (enable) {
            $fsearch.prop('disabled', false);
            $fvGroup.removeClass('dim-filter-value');
        } else {
            $fsearch.prop('disabled', true);
            $fvGroup.addClass('dim-filter-value');
        }
    }

    function loadFilterValueDropdown(filterByValue, filterValue, ignoreTrace) {
        filterValue = filterValue || '';
        var $fsearch = $('#filter-value-dropdown');
        enableDisableFilterDropdown($fsearch, false);
        if (filterByValue !== 'NoFilter') {
            //it is a proper filter val, so get the filter
            $.ajax({
                url: filterApiUrl,
                data: { FilterBy: filterByValue }
            })
                .done(function(indentAndResult) {
                    if (!ignoreTrace) {
                        //Only update the looging if not the main load
                        loggingDisplay.newTrace(indentAndResult.traceIdentifier, indentAndResult.numLogs);
                    }
                    //This removes the existing dropdownlist options
                    $fsearch
                        .find('option')
                        .remove()
                        .end()
                        .append($('<option></option>')
                            .attr('value', '')
                            .text('Select filter...'));

                    indentAndResult.result.forEach(function (arrayElem) {
                        $fsearch.append($("<option></option>")
                            .attr("value", arrayElem.value)
                            .text(arrayElem.text));
                    });
                    $fsearch.val(filterValue);
                    enableDisableFilterDropdown($fsearch, true);
                })
                .fail(function() {
                    alert("error");
                });
        }
    }

    function sendForm(inputElem) {
        var form = $(inputElem).parents('form');
        form.submit();
    }

    //public parts
    return {
        initialise: function(filterByValue, filterValue, exFilterApiUrl) {
            filterApiUrl = exFilterApiUrl;
            loadFilterValueDropdown(filterByValue, filterValue, true);
        },

        sendForm: function(inputElem) {
            sendForm(inputElem);
        },

        filterByHasChanged: function(filterElem) {
            var filterByValue = $(filterElem).find(":selected").val();
            loadFilterValueDropdown(filterByValue);
            if (filterByValue === "0") {
                sendForm(filterElem);
            }
        },

        loadFilterValueDropdown: function(filterByValue, filterValue) {
            loadFilterValueDropdown(filterByValue, filterValue);
        }
    };

}(window.jQuery, LoggingDisplay));