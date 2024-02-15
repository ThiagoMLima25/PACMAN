<style>
  body {
    background-color: #1f1f1f;
    color: white;
  }
</style>

# ᗣ ··· ᗣ ·· Pac-Man ·· ᗣ ··· ᗣ

Recriação do famoso jogo Pac-Man <img src="ImagensProjetoFinalizado\Pac-Man.png" alt="Pac-Man" width="12" height="12" style="transform: scaleX(-1);"> na game engine Unity em C#, que foi lançado pela primeira vez em 1980 pela empresa japonesa Namco.

# ᗣ ·· Jogabilidade
O jogador controla o personagem principal, Pac-Man <img src="ImagensProjetoFinalizado\Pac-Man.png" alt="Pac-Man" width="12" height="12" style="transform: scaleX(-1);">, em um labirinto cheio de pac-dots, além de fugir dos quatro fantasmas coloridos: 

<table>
  <tr>
    <td align="center" style="width: 25%;">
      <img src="ImagensProjetoFinalizado\Blinky.png" alt="Pac-Man" width="15" height="15" style="transform: scaleX(-1);">
    </td>
    <td align="center" style="width: 25%;">
      <img src="ImagensProjetoFinalizado\Inky.png" alt="Pac-Man" width="15" height="15">
    </td>
    <td align="center" style="width: 25%;">
      <img src="ImagensProjetoFinalizado\Clyde.png" alt="Pac-Man" width="15" height="15">
    </td>
    <td align="center" style="width: 25%;">
      <img src="ImagensProjetoFinalizado\Pinky.png" alt="Pac-Man" width="15" height="15">
    </td>
  </tr>
  <tr>
    <td align="center" style="width: 25%;">Blinky</td>
    <td align="center" style="width: 25%;">Inky</td>
    <td align="center" style="width: 25%;">Clyde</td>
    <td align="center" style="width: 25%;">Pinky</td>
  </tr>
</table>

<br/>

O objetivo é comer todos os pontos pac-dots do labirinto enquanto evita ser capturado pelos fantasmas.

# ᗣ ·· Ghost Behaviour
Busquei implementar o comportamento dos fantasmas conforme o game original, tanto no tempo de saída da casa fantasma, movimentos individuais de cada fanstasma quanto no Scatter Mode e Chase mode.
<div style="display: flex; flex-wrap: wrap;">
    <div style="flex: 100%;" align="center">
        <img src="ImagensProjetoFinalizado\ScatterMode.png" alt="Pac-Man">
    </div>
</div>
<div style="display: flex; flex-wrap: wrap;">
    <div style="flex: 100%;" align="center">
        <strong>Scatter Mode</strong>
    </div>
</div>

<br/>

# ᗣ ·· Imagens do Jogo (Play Mode - Unity)
<div style="display: flex; flex-wrap: wrap;">
    <div style="flex: 50%;">
        <img src="ImagensProjetoFinalizado\Unity_Pacman02.png" alt="Pac-Man" width="210">
    </div>
    <div style="flex: 50%;">
        <img src="ImagensProjetoFinalizado\Unity_Pacman03.png" alt="Pac-Man" width="204">
    </div>
    <div style="flex: 50%;">
        <img src="ImagensProjetoFinalizado\Unity_Pacman04.png" alt="Pac-Man" width="209">
    </div>
    <div style="flex: 50%;">
        <img src="ImagensProjetoFinalizado\Unity_Pacman05.png" alt="Pac-Man" width="500">
    </div>
</div>

<br/>

Referência: https://gameinternals.com/understanding-pac-man-ghost-behavior
