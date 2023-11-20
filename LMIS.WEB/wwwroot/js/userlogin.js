

$(function () {

    //Hook up a click event to the login button on the UserLoginModal
    var loginUserButton = $("#UserLoginModal button[name = 'login']").unbind().click(OnUserLoginClick);

    function OnUserLoginClick() {

        var url = "Login/LoginUser";

        var authenticationToken = $("#UserLoginModal input[name='__RequestVerificationToken']").val();

        var username = $("#UserLoginModal input[name='Username']").val();
        var password = $("#UserLoginModal input[name='Password']").val();
        var rememberMe = $("#UserLoginModal input[name='RememberMe']").prop("checked");

        var userInput = {
            __RequestVerificationToken: authenticationToken,
            Username: username,
            Password: password,
            RememberMe: rememberMe
        }

        $.ajax({
            type: 'Post',
          url: path + "/" + url,
          // url: "/" + url,
            data: userInput,
            success: function (data) {

                var parsed = $.parseHTML(data)

                //find errors

               // var hasErrors = $(parsed).find("input[name='LoginInvalid']").val() == "true";
                var hasErrors = data.status == 'failed'

                if (hasErrors == true) {

                    //pass the html data returned from the server to the UserLoginModal

                   // $("#UserLoginModal").html(data)

                    //rewire the login button event

                    //$("#UserLoginModal button[name='login']").unbind().click(OnUserLoginClick)

                    //permit the unobtrusive javascript to work when validation failed

                    //var form = $("#UserLoginModal")

                    //$(form).removeData("validator")
                    //$(form).removeData("unobtrusiveValidation")
                    //$.validator.unobtrusive.parse(form)
                    //location.href = "/dms/"
                    location.href = "/"
                } else {
                    //navigate to the home page               
                   
                    location.href = data.area + "/Home"
                }
            },
            error: function (xhr, ajaxOtions, thrownError) {

                console.error(thrownError + "r\n" + xhr.statusText + "r\n" + xhr.responseText)
            }
        })
    }
})