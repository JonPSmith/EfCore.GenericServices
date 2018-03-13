/**********************************************************************
 * BookList handles the book list, especially the 'filter by' part
 * FOR RAZOR PAGES
 * See http://www.talkingdotnet.com/handle-ajax-requests-in-asp-net-core-razor-pages/
 * First created: 2018/3/12
 * Under the MIT License (MIT)
 * Written by Jon Smith : GitHub JonPSmith, www.thereformedprogrammer.net
 **********************************************************************/

var BookList = (function($) {
    'use strict';

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

    function loadFilterValueDropdown(filterByValue, filterValue) {
        filterValue = filterValue || '';
        var $fsearch = $('#filter-value-dropdown');
        var ajaxSettings = {
            //The Razor pages format is <PageDir>/<Page>?handler=<Last part of method name>
            //In this case it's '/?Handler=FilterSearchContent'
            url: '/?handler=FilterSearchContent',
            data: {
                FilterBy: filterByValue
            }
        };
        if (false) {
            //this make it into a POST - see http://www.talkingdotnet.com/handle-ajax-requests-in-asp-net-core-razor-pages/
            //see also https://docs.microsoft.com/en-us/aspnet/core/security/anti-request-forgery#javascript-ajax-and-spas
            ajaxSettings.type = 'POST';
            ajaxSettings.beforeSend = function(xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            };
        }

        enableDisableFilterDropdown($fsearch, false);
        if (filterByValue !== 'NoFilter') {
            //it is a proper filter val, so get the filter
            $.ajax(ajaxSettings)
            .done(function(result) {
                //This removes the existing dropdownlist options
                $fsearch
                    .find('option')
                    .remove()
                    .end()
                    .append($('<option></option>')
                        .attr('value', '')
                        .text('Select filter...'));

                result.forEach(function (arrayElem) {
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
        initialise: function(filterByValue, filterValue) {
            loadFilterValueDropdown(filterByValue, filterValue);
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

}(window.jQuery));
// Write your Javascript code.