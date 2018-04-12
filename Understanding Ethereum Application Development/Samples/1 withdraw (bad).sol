pragma solidity ^0.4.0;

contract Superbowl {
    struct Winner {
        address winnerAddress;
        uint amount;
    }

    Winner[] winnings;

    function disperseWinnings() public {
        // Not shown: ensuring the caller has permission to disperse the winnings (e.g., Owner pattern)

        // Bad way to disperse winnings: push to multiple accounts.
        for (uint i = 0; i != winnings.length; ++i) {
            winnings[i].winnerAddress.transfer(winnings[i].amount);
        }
    }
}

contract BadWinner {
    function () public {
        revert();
    }
}