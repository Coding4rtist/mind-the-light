# mind-the-light
Mind the light is a multiplayer 3D game made in Unity.


### The (Intended) Schedule

1. **Game Design**: GDD (Game Design Document), Diagrams
2. **Basic Architecture**: Design the "engine" and the main components. 
3. **Player Interaction**:  Implement all core interactivity for the player - moving around, attacking things, opening doors, dying, picking up and using inventory. Bare-Bones representative objects in the environment will be created to test the interactivity 
4. **Finish the prototype**: Add sound and graphic. Playable version of the prototype
5. **Making the World Active**: Add the AI, game "events", traps, and special effects. By the end of this period, the game should be a pretty complete tech-demo of all of the game's major features. 
6. **Adding Content and Rules**: Take the project from "tech demo" to game. Add all additional content. Complete and balance the game-play mechanics. Apply polish where time permits - adding special effects, animation, etc. Add GUI and menu.
7. **Testing and Game Release**: Fix bugs (**no adding features!**) Package up the game and release it. Finish documentation. 



### The Idea

meccanica principale: illuminazione dinamica torcia

meccaniche secondarie: 

- abilità personaggi
- shooting guardia
- velocità dinamica ladro
- oggetti da rubare posizionati random

extra: multiplayer, mappa generata proceduralmente

### Bilanciamento giocatori:

**Guardia**:

[-] non conosce la posizione degli oggetti
[-] vede solo nell'area illuminata dalla torcia (e a 360 gradi in un raggio minore ?)
[+] può sparare (attacco dalla distanza) il ladro
[+] può piazzare delle trappole che rallentano / aiutano a trovare il ladro
[+] può usare degli oggetti per attirare l'attenzione della guardia

**Ladro**:
[-] non conosce la posizione degli oggetti
[+] riesce a vedere nel buio
[-] più oggetti trasporta e piů diventa lento
[+] può nascondersi in alcuni punti
[-] può scappare solo se ha rubato un tot di oggetti

extra:

- alcune porte sono bloccate per il ladro (richiedono chiave), ma non per la guardia. Se la guardia non richiude le porte il ladro può passare
- il passaggio del ladro in alcuni punti può fare rumore ed attirare la guardia (es. cespugli)


wewe


ciao