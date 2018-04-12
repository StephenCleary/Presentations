pragma solidity ^0.4.0;

contract Superbowl {
    struct Winner {
        address winnerAddress;
        uint amount;
    }

    Winner[] winnings;
    mapping(address => uint) winningsIndexByAddress;

    function getMyWinnings() public {
        Winner winner = winnings[winningsIndexByAddress[msg.sender]];
        if (winner.amount != 0) {
            uint amount = winner.amount;
            winner.amount = 0;
            msg.sender.transfer(amount);
        }
    }
}
