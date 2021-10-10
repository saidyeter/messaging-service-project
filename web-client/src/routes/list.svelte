<script>
    import { each, onMount } from "svelte/internal";
    import { fade, fly } from "svelte/transition";

    import { connect } from "../../scripts/socket-helper";
    import { authStore, opponentList } from "../store";
    import { apiGetOpponents } from "../../scripts/api-helper";
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

    function goChat(event) {
        //console.log(event.target);
        opponenUserName = event.target.id;
        if (opponenUserName) {
            chatbox.startFetching(opponenUserName);
            shown = false;
        } else {
            console.log(event.target);
        }
        //setTimeout(goChat,10000)
    }
    function goList(event) {
        shown = true;
        opponenUserName = "";
    }
</script>

{#if shown}
    <div class="contact-list" trasition:fly>
        {#each $opponentList as contact}
            <div class="contact-item" id={contact.userName} on:click={goChat}>
                {contact.displayName} : {contact.messageSummary}
                <div class="last-message" id={contact.userName}>
                    {contact.userName} Last Messaged : {contact.lastMessaged}
                </div>
            </div>
        {/each}
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
</style>
