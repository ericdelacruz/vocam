

/**
 * Created by saan on 10/6/13.
 */
var TS_AJAX_FORM ={

    /*ERROR Message Display Element Reference*/
    MY_MESSAGE_ERR : $(".my-message-error"),

    /*SUCCESS Message Display Element Reference*/
    MY_MESSAGE_SUC : $(".my-message-success"),

    /*Shows the input message and hides it in 5 seconds*/
    showMessage:function(msg, type){

        var message = (type === 'ERR')? this.MY_MESSAGE_ERR : this.MY_MESSAGE_SUC,

            txt = $(message).find('a');

        $(txt).html(msg);

        message.fadeIn('fast',function(){

            message.fadeOut(5000);
        })
    },
    /*
     *Handler: success, Once the form is submitted and response
     *arrives, it will be activated.
     */
    successHandler: function(responseText, statusText, xhr, form){

        TS_AJAX_FORM.showMessage("Form Submitted Status("+statusText+").", "SUC");
    },
    /*
     *responseText, statusText, xhr, $form: beforeSubmit, for validation   Let, title and roll is your required field.
     *Let's show an error message if these fields are blank.
     */
    beforeSubmitHandler:function(arr, form, options){

        var isValid = true;

        $.each(arr,function(index, aField){

            if('name' === aField.name && aField.value === ""){

                TS_AJAX_FORM.showMessage("Name Can not be Empty.", "ERR");

                isValid = false;

            }else if('roll' === aField.name && aField.value === ""){

                TS_AJAX_FORM.showMessage("Roll Can not be Empty.", "ERR");

                isValid = false;
            }
        });
        return isValid;
    },
    /*Initializing Ajax Form*/
    initMyAjaxForm:function(){

        $("#my-detail-form").ajaxForm({

            beforeSubmit:this.beforeSubmitHandler,

            success:this.successHandler,

            clearForm:true
        });
    }
};
/*My Small Tutorial: Execution point*/
$(document).ready(function(){
    TS_AJAX_FORM.initMyAjaxForm()
});

