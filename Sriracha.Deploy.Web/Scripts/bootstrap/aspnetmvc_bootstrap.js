jQuery.validator.setDefaults({
    highlight: function (element, errorClass, validClass) {
        if (element.type === 'radio') {
            this.findByName(element.name).addClass(errorClass).removeClass(validClass);
        } else {
            $(element).addClass(errorClass).removeClass(validClass);
            $(element).closest('.control-group').removeClass('success').addClass('error');
        }
    },
    unhighlight: function (element, errorClass, validClass) {
        if (element.type === 'radio') {
            this.findByName(element.name).removeClass(errorClass).addClass(validClass);
        } else {
            $(element).removeClass(errorClass).addClass(validClass);
            $(element).closest('.control-group').removeClass('error').addClass('success');
        }
    }
});

$(function () {
    $("span.field-validation-valid, span.field-validation-error").addClass('help-inline');
    $("span.field-validation-error").css("color","red")
    $("div.validation-summary-errors").has("li:visible").addClass("alert alert-block alert-error");
    $("div.validation-summary-errors span").addClass("alert alert-block alert-error");

});
