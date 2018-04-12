pragma solidity ^0.4.0;

contract Callback {
    // Assumption: callMe is "good" for the receiver
    function callMe() public;
}

contract MyContract {
    bool callbackInvoked;
    Callback callback;

    function invokeCallback() public {
        require(!callbackInvoked);
        callback.callMe();
        callbackInvoked = true;
    }
}
