    $(function () {
    $(".anchorDetail").click(function () {
        debugger;
        var $buttonClicked = $(this);
        var PostBackURL = $buttonClicked.attr('data-url');
        var id = $buttonClicked.attr('data-id');
        var options = { "backdrop": "static", keyboard: true };
        $.ajax({
            type: "GET",
            url: PostBackURL,
            contentType: "application/json; charset=utf-8",
            data: { "id": id },
            datatype: "json",
            success: function (data) {
                debugger;
                $('#myModalContent').html(data);
                $('#myModal').modal(options);
                $('#myModal').modal('show');
            },
            error: function () {
                alert("Dynamic content load failed.");
            }
        });
    });
    //$("#closebtn").on('click',function(){
    // $('#myModal').modal('hide');
    $("#closbtn").click(function () {
        $('#myModal').modal('hide');
    });
});