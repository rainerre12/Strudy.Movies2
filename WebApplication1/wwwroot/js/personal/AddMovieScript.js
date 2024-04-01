

$(function () {

    $('#registerMovie').click(function () {
        // Perform form validation here if needed
        var isValid = true; // Assume form is valid by default

        // Example validation (check if the required fields are filled)
        if ($('#inputmoviename').val().trim() === '') {
            isValid = false;
            $('#inputmoviename').addClass('is-invalid');
        } else {
            $('#inputmoviename').removeClass('is-invalid');
        }

        if ($('#selectedGenreId').val() === '') {
            isValid = false;
            $('#selectedGenreId').addClass('is-invalid');
        } else {
            $('#selectedGenreId').removeClass('is-invalid');
        }

        if (isValid) {

            var MovieName = $('#inputmoviename').val();
            var Genreid = $('#selectedGenreId').val();

            var data = {
                Movies: {
                    Name: MovieName,
                    Type: Genreid
                }
            };

            $.ajax({
                url: '/Home/PostRegisterMovie',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (response) {
                    toastr.success('Movie Registered');
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
                        toastr.error('Error occurred while registering Movie');
                    }
                }
               
            });
        }
    });

    //$('.toastrDefaultSuccess').click(function () {
    //    toastr.success('Movie Registered')
    //});


});