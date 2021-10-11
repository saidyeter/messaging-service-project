<script>
    import { each, onMount } from "svelte/internal";
    import { fade, fly } from "svelte/transition";

    import { getNotificationsContext } from "svelte-notifications";
    const { addNotification } = getNotificationsContext();
    import { connect } from "../helper/socket-helper.js";
    import { authStore, opponentList } from "../store";
    import { apiGetOpponents, apiGetUserInfo } from "../helper/api-helper.js";
    import Chat from "./chat.svelte";
    let chatbox;
    let opponenUserName = "";
    onMount(() => {
        getOpponents();
        connect($authStore, onNewMessage, reConnect);
    });

    function reConnect() {
        if ($authStore) {
            connect($authStore, onNewMessage, reConnect);
        }
    }

    function redirectToLogin() {
        authStore.set("");
    }
    async function getOpponents() {
        const opponents = await apiGetOpponents($authStore);
        $opponentList = [
            ...opponents.sort((a, b) =>
                a.lastMessaged > b.lastMessaged ? -1 : 1
            ),
        ];
    }
    async function onNewMessage(msg) {
        //console.log(`new message :`, msg.data);
        if (msg.data.includes("invalid token")) {
            redirectToLogin();
        }

        if (!shown) {
            chatbox.onNewMessage(JSON.parse(msg.data));
        }
        getOpponents();
    }

    let shown = true;

    function openChat(event) {
        opponenUserName = event.target.id;
        goChat();
    }

    function goChat() {
        chatbox.startFetching(opponenUserName);
        shown = false;
    }
    function goList(event) {
        shown = true;
        opponenUserName = "";
    }

    let opponent="";

    async function handleNewMessage(event) {
        if (opponent.length<4) {            
            addNotification({
                text: "User couldnt found",
                type: "danger",
                position: "top-right",
                removeAfter: 2000,
            });
            return
        }
        try {
            const userdata = await apiGetUserInfo($authStore, opponent);
            if (userdata) {
                opponenUserName = opponent;
                opponent = "";
                goChat();
            }
        } catch (error) {
            addNotification({
                text: error.toString(),
                type: "danger",
                position: "top-right",
                removeAfter: 2000,
            });
        }
    }
</script>

{#if shown}
    <div class="contact-list" trasition:fly>
        {#each $opponentList as contact}
            <div class="contact-item" id={contact.userName} on:click={openChat}>
                {contact.displayName} : {contact.messageSummary}
                <div class="last-message" id={contact.userName}>
                    {contact.userName} Last Messaged : {contact.lastMessaged}
                </div>
            </div>
        {/each}
        <div class="new-message">
            <input
                type="text"
                bind:value={opponent}
                placeholder="username for new message"
            />
            <button on:click={handleNewMessage}>New</button>
        </div>
    </div>
{/if}
<Chat bind:this={chatbox} on:goList={goList} />

<style>
    .last-message {
        font-size: 0.5rem;
    }
    .contact-list {
        text-align: center;
        padding: 0em;
        max-width: 320px;
        margin: 0 auto;
    }
    .contact-item {
        width: auto;
        background-color: rgb(0 0 0 / 10%);
        padding: 0.5em;
        margin-top: 0.5em;
        margin-bottom: 0.5em;
        border-radius: 0.5em;
        text-align: left;
        cursor: pointer;
    }
    .new-message{
        display: flex;
        justify-content: space-between;
    }
    input{
        width: 100%;
    }
</style>
