angular.module("umbraco").controller("KeyValueEditorDialogController", function ($scope) {

    if (!$scope.dialogData.key)
        $scope.dialogData.key = null;
    if (!$scope.dialogData.value)
        $scope.dialogData.value = null;

});