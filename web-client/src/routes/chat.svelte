<script>
    import { fade, fly, blur } from "svelte/transition";
  	import  { getNotificationsContext } from 'svelte-notifications';
	const { addNotification } = getNotificationsContext();
    import {
        apiGetLastMessageId,
        apiGetOlderMessagesFrom,
        apiGetSingleMessage,
        apiSendMessage,
    } from "$lib/helper/api-helper.js";


    import { createEventDispatcher, onMount } from "svelte";
    import { messageList, currentUser } from "$lib/helper/store";

    const dispatch = createEventDispatcher();

	import MsgItem from "$lib/components/msg-item.svelte";

    let username = "";
    let shown = false;

    let newmessage;

    export function startFetching(un) {
        console.log("fetching data user :", un);
        username = un;
        shown = true;
        $messageList = [];
        initial();
    }

    export async function onNewMessage(data) {
        if (data.sender == username) {
            const msg = await apiGetSingleMessage(data.messageId);
            //console.log("yeni mesaj geldi",data,msg);
            $messageList = [...$messageList, msg];
            scrollSync()
        } else {
            addNotification({
                text: "new message from " + data.sender,
                position: "top-right",
                removeAfter: 2000,
            });
        }
    }
    //

    async function initial() {
        const latestMessage = await apiGetLastMessageId(username);
        console.log("latest ok");
        const moreMessages = await more(latestMessage.messageId);
        moreMessages.messageIdList = [
            latestMessage.messageId,
            ...moreMessages.messageIdList,
        ];
        //console.log("more ok", moreMessages);
        const messages = await Promise.all(
            moreMessages.messageIdList
                .map(async (x) => {
                    const msg = await apiGetSingleMessage(x);
                    //console.log(msg);
                    return msg;
                })
                .reverse()
        );

        $messageList = [...messages];
        scrollSync()
    }

    function more(messageId) {
        return apiGetOlderMessagesFrom(messageId);
    }

    function goList() {
        dispatch("goList");
        shown = false;
    }


    async function sendMessage(event) {
        const msg = {
            receiverUser: username,
            senderUser: $currentUser,
            message: newmessage,
        };

        try {
            await apiSendMessage(username, newmessage);

            $messageList = [...$messageList, msg];
            newmessage = "";
            scrollSync()
        } catch (error) {
            addNotification({
                text: "message couldnt send : " + error,
                type: "danger",
                position: "top-right",
                removeAfter: 2000,
            });
        }
    }

    function scrollSync() {
        var elem = document.getElementById("msglist");
        if (elem) {           
            elem.scroll(0,elem.scrollHeight)
        }
    }

    //scroll up
</script>

{#if shown}
    <div class="chat-box-wrapper" transition:fly>
        <div class="chat-header">
            <div class="back" on:click={goList}>&lt;back</div>
            <div class="opponent">{username}</div>
        </div>

        <div class="chat-messages" id="msglist">
            <ul>
                {#each $messageList as msg}
                    <li>
                        <MsgItem messageData={msg}/>                      
                    </li>
                {/each}
            </ul>
        </div>
        <div class="chat-send-message">
            <input type="text" bind:value={newmessage} />
            <button on:click={sendMessage}>send</button>
        </div>
    </div>
{/if}

<style>
    .chat-box-wrapper {
        background-color: rgb(0 0 0 / 10%);
        display: flex;
        flex-direction: column;
        margin: 0 auto;
        width: 20em;
        height: 95vh;
    }
    .chat-messages {
        padding: 1em;
        overflow-y: auto;
        flex-grow: 1;
    }
    .chat-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        padding: 1em;
        flex-basis: 2em;
        flex-shrink: 0;
    }
    .chat-send-message {
        padding: 1em;
        flex-basis: 1em;
        flex-shrink: 0;
        display: flex;
    }
   
    .back {
        cursor: pointer;
        color: white;
    }
    .opponent {
        color: white;
    }
    ul {
        list-style: none;
        padding-left: 0;
        bottom: 0;
    }
    button{
        width: 5em;
    }
</style>
