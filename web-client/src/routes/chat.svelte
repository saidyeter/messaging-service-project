<script>
    import { fade, fly, blur } from "svelte/transition";
    import { getNotificationsContext } from "svelte-notifications";
    import {
        apiGetLastMessageId,
        apiGetOlderMessagesFrom,
        apiGetSingleMessage,
        apiSendMessage,
    } from "../../scripts/api-helper";
    let username = "";
    let shown = false;
    export function startFetching(un) {
        //console.log("fetching data user :", un);
        username = un;
        shown = true;
        $messageList = [];
        initial();
    }

    export async function onNewMessage(data) {
        if (data.sender == username) {
            const msg = await apiGetSingleMessage($authStore, data.messageId);
            //console.log("yeni mesaj geldi",data,msg);
            $messageList = [...$messageList, msg];
        }
    }
    //

    async function initial() {
        const latestMessage = await apiGetLastMessageId($authStore, username);
        //console.log("latest ok");
        const moreMessages = await more(latestMessage.messageId);
        moreMessages.messageIdList = [
            latestMessage.messageId,
            ...moreMessages.messageIdList,
        ];
        //console.log("more ok", moreMessages);
        const messages = await Promise.all(
            moreMessages.messageIdList
                .map(async (x) => {
                    const msg = await apiGetSingleMessage($authStore, x);
                    //console.log(msg);
                    return msg;
                })
                .reverse()
        );

        $messageList = [...messages];
    }

    function more(messageId) {
        return apiGetOlderMessagesFrom($authStore, messageId);
    }

    const { addNotification } = getNotificationsContext();
    import { createEventDispatcher, onMount } from "svelte";
    import { authStore, messageList,currentUser } from "../store";

    const dispatch = createEventDispatcher();

    function goList() {
        dispatch("goList");
        shown = false;
    }

    let newmessage;

    async function sendMessage(event) {
        try {
            await apiSendMessage($authStore, username, newmessage);

            $messageList = [
                ...$messageList,
                {
                    receiverUser: username,
                    senderUser: $currentUser,
                    message: newmessage,
                },
            ];
            newmessage = "";
        } catch (error) {
            addNotification({
                text: "message couldnt send : " + error,
                type: "danger",
                position: "top-right",
                removeAfter: 2000,
            });
        }
    }

    //initial
    //scroll up
    //send
    //ilerde socket ile
</script>

{#if shown}
    <div class="chat-box-wrapper" transition:fly>
        <div class="chat-header">
            <div class="back" on:click={goList}>&lt;back</div>
            <div class="opponent">{username}</div>
        </div>

        <div class="chat-messages">
            <ul>
                {#each $messageList as msg}
                    <li>
                        <div class="msg-item">
                            {#if msg.senderUser == username}
                                <div class="received">
                                    <div class="sender">
                                        {msg.senderUser}
                                    </div>
                                    {msg.message}
                                </div>
                            {:else}
                                <div class="sended">
                                    {msg.message}
                                </div>
                            {/if}
                        </div>
                    </li>
                {/each}
            </ul>
        </div>
        <div class="chat-send-message">
            <div class="input">
                <input type="text" bind:value={newmessage} />
            </div>
            <div class="submit">
                <button on:click={sendMessage}>send</button>
            </div>
        </div>
    </div>
{/if}

<style>
    .chat-box-wrapper {
        background-color: rgb(0 0 0 / 10%);
        text-align: center;

        max-width: 320px;
        margin: 0 auto;
        height: 100vh;
    }
    .chat-messages {
        overflow-y: auto;
        height: 70vh;
        padding: 1em;
    }
    .chat-header {
        display: flex;
        flex-direction: row;
        justify-content: space-between;
        height: 5vh;
        padding: 1em;
    }
    .chat-send-message {
        height: 5vh;
        display: flex;
        padding: 1em;
    }
    input {
        width: 100%;
    }
    .sended {
        text-align: right;
    }
    .received {
        text-align: left;
    }
    .back {
        cursor: pointer;
        color: white;
    }
    .opponent {
        color: white;
    }
    li + li {
        margin-top: 0.5em;
    }
    ul {
        list-style: none;
        padding-left: 0;
        bottom: 0;
    }
    .sender {
        font-size: 0.5rem;
    }
</style>
