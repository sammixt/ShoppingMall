$(document).ready(function () {
    $("#submit").click(function () {

        var catname = $("#CatValue").val();

        if (catname != "") {
            var CategoryViewModel = {
                CategoryName: catname
            }
            $.ajax({
                url: '/Category/AddCat',
                contentType: 'application/json; charset=utf-8',
                type: 'POST',
                data: JSON.stringify(CategoryViewModel)

            })
            .success(function (result) {
                if (result == "inserted") {
                    window.location.reload();
                } else if (result == "exist") {
                    $("#ErrorMsg").text("The Category Already Exist")
                } else {
                    $("#ErrorMsg").text("An Error Occured")
                }
            })
            .error(function (xhr, status) {
                $('#count').text('0');
            })
        } else {
            $("#ErrorMsg").text("Please Enter a Valid Name")
        }

    })
})
function delCart(id) {
    var CategoryViewModel = {
        CategoryId: id
    }
    $.ajax({
        url: '/Category/DelCat',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        data: JSON.stringify(CategoryViewModel)

    })
        .success(function (result) {
            if (result == true) {
                window.location.reload();
            } else {
                $("#ErrorMsg").text("An Error Occured")
            }
        })
        .error(function (xhr, status) {
            $('#count').text('0');
        })
}

function reAddCart(id) {
    var CategoryViewModel = {
        CategoryId: id
    }
    $.ajax({
        url: '/Category/reAddCat',
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        data: JSON.stringify(CategoryViewModel)

    })
        .success(function (result) {
            if (result == true) {
                window.location.reload();
            } else {
                $("#ErrorMsg").text("An Error Occured")
            }
        })
        .error(function (xhr, status) {
            $('#count').text('0');
        })
}

function enabler(id) {
    $("#catVal_" + id).prop("disabled", false);
    $("#catVal_" + id).css('border', 'solid');
    $("#catVal_" + id).css('border-color', '#00adef');
    $("#enabler_" + id).css('display', 'none')
    $("#updater_" + id).css('display', 'block')
}

function updater(id) {
    var catname = $("#catVal_" + id).val();
    if (catname != "") {
        var CategoryViewModel = {
            CategoryId: id,
            CategoryName: catname
        }
        $.ajax({
            url: '/Category/UpdateCat',
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            data: JSON.stringify(CategoryViewModel)

        })
            .success(function (result) {
                if (result == "inserted") {
                    window.location.reload();
                } else if (result == "exist") {
                    $("#errormsg_" + id).text("The Category Already Exist")
                } else {
                    $("#errormsg_" + id).text("An Error Occured")
                }
            })
            .error(function (xhr, status) {
                $('#count').text('0');
            })
    } else {
        $("#errormsg_" + id).text("Field cannot be empty");
    }
}