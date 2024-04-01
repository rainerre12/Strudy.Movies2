$(function () {
    $('#registerUser').click(function () {
        var isValid = true;

        if ($('#inputFirstName').val().trim() === '') {
            isValid = false;
            $('#inputFirstName').addClass('is-invalid');
        } else {
            $('#inputFirstName').removeClass('is-invalid');
        }

        if ($('#inputLastName').val().trim() === '') {
            isValid = false;
            $('#inputLastName').addClass('is-invalid');
        } else {
            $('#inputLastName').removeClass('is-invalid');
        }

        if ($('#inputUserName').val().trim() === '') {
            isValid = false;
            $('#inputUserName').addClass('is-invalid');
        } else {
            $('#inputUserName').removeClass('is-invalid');
        }

        if ($('#inputUserPassword').val().trim() === '') {
            isValid = false;
            $('#inputUserPassword').addClass('is-invalid');
        } else {
            $('#inputUserPassword').removeClass('is-invalid');
        }


        if (isValid) {
            var Firstname = $('#inputFirstName').val();
            var Lastname = $('#inputLastName').val();
            var Username = $('#inputUserName').val();
            var Password = $('#inputUserPassword').val();
            var hasprivelage = $('#Checkisadmin').is(':checked');



            var data = {
                Persons: {
                    FirstName: Firstname,
                    LastName: Lastname
                },
                UserAccounts: {
                    Username: Username,
                    Userpassword: Password,
                    hasPrivelage: hasprivelage
                }
            };


            $.ajax({
                url: '/Home/PostRegisterUser',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    toastr.success('User Registered');
                    setTimeout(function () {
                        window.location.href = '/Home/Index'; // Redirect to the same page after a delay
                    }, 2000);
                },
                error: function (xhr, status, error) {
                    //if (xhr.status = 400) {
                    //    alert('Movie with the same name already exists.');
                    var errorMessage = xhr.responseJSON && xhr.responseJSON.message;
                    if (errorMessage) {
                        toastr.error(errorMessage);
                    } else {
                        console.error('Error:', error);
                        toastr.error('Error occurred while registering user');
                    }
                }

            });

        }



    });
});