<script>
    /*
   {
        "id":"6162eec8f70aeab52c62c560",
        "senderUser":"seval",
        "receiverUser":"said",
        "message":"selam",
        "sendedAt":"2021-10-10T13:46:48.795Z",
        "seenAt":"2021-10-10T13:46:48.795Z"
    }
   */
    export let messageData;
    import { currentUser } from "../store";
    import {relativeDate} from "../helper/date-helper"
</script>

<div class="msg-item">
    {#if messageData.receiverUser == $currentUser}
        <div class="msg-box received">
            <div class="sender">
                {messageData.senderUser}
            </div>
            {messageData.message}
            <div class="msg-date">
                send:{relativeDate(messageData.sendedAt)}
            </div>
        </div>
    {:else}
        <div class="msg-box sended">
            {messageData.message}
            
            <div class="msg-date">
                send:{relativeDate(messageData.sendedAt)} 
                seen:{relativeDate(messageData.seenAt)}
            </div>
            <div class="msg-date">
            </div>
        </div>
    {/if}
</div>

<style>
    .sended {
        text-align: right;
        background-color: rgb(132 169 203);
    }
    .received {
        text-align: left;
        background-color: rgb(118 167 99);
    }
    .sender{
        font-size: 0.5rem;
    }
    .msg-date {
        font-size: 0.5rem;
        text-align: right;
    }
    .msg-box {
        color:white;
        padding: 0.5em 0.5em;
        line-height: 1em;
        border-radius: 7px;
        margin-bottom: 1em;
        /* width: 90%;
        position: relative; 
        $green: #86BB71;
$blue: #94C2ED;
*/
    }
</style>
