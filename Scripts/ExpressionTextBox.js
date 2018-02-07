﻿ko.bindingHandlers["cc-ExpressionTextBox"] = {

    init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        $(element).change(function() {

            var input = $(this);
            var value = input.val();

            // if the value in the input changes, check whether it is not a math expression
            if (value.match(/^[0-9.,\-\+\*\/\s\(\)]+$/)) {
                try {
                    var result = eval(value.replace(/,/g, "."));
                    if (typeof result === "number") {
                        // update the field value
                        var resultString = dotvvm.globalize.formatString("n2", result);
                        if (value !== resultString) {
                            input.val(resultString);
                        }
                    }
                }
                catch (e) {
                }
            }
        });

    },
    before: ["value"]
};