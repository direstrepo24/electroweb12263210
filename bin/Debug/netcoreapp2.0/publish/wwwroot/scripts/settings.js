

/*$(document).ready(function(){
   // alert("Click");

   
    $(document).on("click","#consultar",function() {

        alert("hola");
        //$("#load_").slideDown('slow');
    });


    function Consultar(){
        alert("hola");
    }



    

    function getElementById(result){
        console.log(result);
    }


  

});*/
$(function() {
   

    
dotvvm.events.init.subscribe(function () {
    dotvvm.postBackHandlers["confirm"] = function ConfirmPostBackHandlerCustom(options) {
        this.execute = function(callback) {
            alert("hola");
            console.log("hola");
            // set the message to the confirmation dialog
           /// $("#confirm-dialog p.message").text(options.message);
                        
            // display the confirmation dialog
            //$("#confirm-dialog").show();

            // do the postback when the OK button is clicked
            // (unbind first because a previous callback could be attached to the event)
           // $("#confirm-dialog button.ok-button").unbind("click").on("click", callback);
        };
    };
});

});