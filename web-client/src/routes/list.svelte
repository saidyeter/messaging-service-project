<script>
    import { each, onMount } from "svelte/internal";
    import { fade, fly } from "svelte/transition";

    import { connect } from "../../scripts/socket-helper";
    import { authStore, opponentList } from "../store";
    import { apiGetOpponents } from "../../scripts/api-helper";
    import Chat from "./chat.svelte";
    let chatbox;
    let opponenUserName = ""
    onMount(() => {
        initial();
        connect($authStore,onNewMessage)
    });
    async function initial() {
        const opponents = await apiGetOpponents($authStore);
        $opponentList = [...opponents];
    }
    async function onNewMessage(msg) {
        console.log(`new message :`,msg.data);
        if (opponenUserName == "") {
            //bildirim çıkar 
            //göndericiyi listede başa çıkart
        }
        else{
            chatbox.onNewMessage(JSON.parse(msg.data));
        }
    } 

    let shown = true;

    function goChat(event) {
        //console.log(event.target);
        opponenUserName = event.target.id
        chatbox.startFetching(opponenUserName);
        shown = false;
    }
    function goList(event) {
        shown = true;
        opponenUserName= ""
    }
</script>

{#if shown}
    <div class="contact-list" trasition:fly>
        {#each $opponentList as contact}
            <div class="contact-item" id={contact.userName} on:click={goChat}>
                {contact.displayName}
                <div class="last-message">
                    {contact.lastLogin}
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
