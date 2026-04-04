# Subnet Mask Calculator (WPF)

WPF-käyttöliittymällä varustettu IPv4-aliverkkolaskin. Projekti on tehty kurssilla **Tietoliikenteen perusteet**, ja sen päätavoitteena oli harjoitella IP-osoitteisiin liittyviä laskutoimituksia **bitwise-operaattoreilla** (shiftit, AND/OR/NOT).

Syötät ohjelmaan:
- IPv4-osoitteen (esim. `192.168.1.10`)
- aliverkon pituuden (CIDR-prefix, esim. `24`)

…ja ohjelma laskee näiden perusteella:
- aliverkon peitteen (Subnet Mask)
- verkko-osoitteen (Network Address)
- ensimmäisen käyttökelpoisen osoitteen (First Usable)
- viimeisen käyttökelpoisen osoitteen (Last Usable)
- broadcast-osoitteen (Broadcast Address)
- käytettävissä olevien host-osoitteiden määrän (Max. available hosts)

> Toteutus kehitettiin aluksi konsolisovelluksena ja myöhemmin sille tehtiin käyttöliittymä WPF:llä.

---

## Teknologiat

- **.NET 8** (`net8.0-windows`)
- **C#**
- **WPF** (Windows-työpöytäsovellus)

---

## Käyttö

1. Kirjoita `Enter IP Address` -kenttään IPv4-osoite muodossa `a.b.c.d`.
2. Kirjoita `Enter Subnet Prefix` -kenttään prefix väliltä `0–32`.
3. Paina **Calculate**.
4. Tyhjennä kentät painamalla **Clear**.

Syötteet validoidaan:
- IP-osoite tarkistetaan regexillä (0–255 jokaisessa oktetissa)
- prefixin tulee olla kokonaisluku väliltä `0–32`

---

## Kääntäminen ja ajaminen

### Visual Studio (suositus)
1. Avaa ratkaisu/projekti Visual Studiolla.
2. Buildaa ja käynnistä (F5).

### .NET CLI
Aja repositorion juuressa:

- Build:
  - `dotnet build`
- Run:
  - `dotnet run --project "Subnet Mask Calculator with GUI/Subnet Mask Calculator with GUI.csproj"`

---

## Miten laskenta toimii (tiivistetysti)

Sovellus muuntaa IP-osoitteen neljä oktettia 32-bittiseksi luvuksi ja käyttää bitwise-operaatioita:

- Subnet mask luodaan siirtämällä `uint.MaxValue`-arvoa vasemmalle:  
  `mask = uint.MaxValue << (32 - prefixLength)`
- Network address:  
  `network = ip & mask`
- Broadcast address:  
  `broadcast = network | ~mask`
- Ensimmäinen käyttökelpoinen: `network + 1`
- Viimeinen käyttökelpoinen: `broadcast - 1`
- Hostien määrä: `2^(32 - prefix) - 2`

---

## Oppimistavoitteet (miksi tämä projekti tehtiin)

Tämän työn kautta harjoiteltiin mm.:

- IPv4-osoitteen rakennetta ja CIDR-prefixiä käytännössä
- bittiesitystä ja bitwise-operaattoreita (`<<`, `&`, `|`, `~`)
- syötteiden validointia (regex + rajatarkistukset)
- WPF:n perusteita (XAML, event handlerit, UI:n päivittäminen)
- yksinkertaisen “konsolista GUI:hin” -siirtymän ajattelutapaa (logiikka + käyttöliittymä)

---

## Tunnetut rajoitteet / huomioita

- Sovellus on IPv4-painotteinen (ei IPv6-tukea).
- Prefixeillä `/31` ja `/32` hostien laskenta ei vastaa kaikkia käytännön erikoistapauksia (esim. point-to-point `/31`), koska kaava vähentää aina 2 (network + broadcast).

---

## Tulevaisuuden kehitysideoita

- Korjaa `/31` ja `/32` -erikoistapaukset host-laskennassa ja “usable”-osoitteissa.
- Näytä tulokset myös binäärimuodossa (IP, mask, network).
- Lisää “Copy to clipboard” -painikkeet tulosriveille.
- Lisää IPv6-tuki (ainakin perusprefix-laskenta).
- Erottele laskentalogiikka omaan luokkaan/kirjastoon ja lisää yksikkötestit (esim. MSTest/xUnit).

---

## Virustorjunnan “false positive” -huomio

Monet virustorjuntaohjelmat voivat tunnistaa tämän ohjelman **virheellisesti haittaohjelmaksi** ja siirtää sen karanteeniin. Tämä projekti on kuitenkin **vaaraton laskinsovellus**, joka tekee vain paikallisia laskutoimituksia käyttäjän syötteistä.

Jos kohtaat tämän:
- suosi ajamista **kääntämällä itse lähdekoodista**
- tarkista lähdekoodi (tämän repositorion sisältö) ennen ajamista
- tarvittaessa lisää omaan virustorjuntaan poikkeus käännetylle `.exe`-tiedostolle tai projektikansiolle
- jatkokehitysidea: julkaisu allekirjoitettuna (code signing) ja/tai ilmoitus AV-toimittajille false positive -tapauksista
