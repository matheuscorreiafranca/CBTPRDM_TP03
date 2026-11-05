"""Gera os SVGs vetoriais originais do Rastro. Execute na raiz do repositório."""
from pathlib import Path

root = Path(__file__).resolve().parents[1] / "src/Rastro.App/Resources"
images = root / "Images"
images.mkdir(exist_ok=True)

def svg(name, content, size=24, color="#78838E"):
    (images / f"{name}.svg").write_text(
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{size}" height="{size}" viewBox="0 0 24 24">'
        f'<g fill="none" stroke="{color}" stroke-width="1.65" stroke-linecap="round" stroke-linejoin="round">{content}</g></svg>', encoding="utf-8")

svg("nav_grid", '<rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/>', color="#D85B28")
svg("nav_box", '<path d="m12 3 9 5v9l-9 5-9-5V8zM3 8l9 5 9-5M12 13v9M7.5 5.5l9 5"/>')
svg("nav_help", '<circle cx="12" cy="12" r="9"/><path d="M9.5 9a2.5 2.5 0 0 1 5 .5c0 2-2.5 2-2.5 4M12 17h.01"/>')
svg("search", '<circle cx="10.5" cy="10.5" r="6.5"/><path d="m16 16 4.5 4.5"/>')
svg("truck", '<path d="M2 5h13v12H2zM15 9h4l3 4v4h-7"/><circle cx="6" cy="18" r="2" fill="#FEF0E8"/><circle cx="18" cy="18" r="2" fill="#FEF0E8"/><path d="M15 13h7"/>', color="#D85B28")
svg("check", '<circle cx="12" cy="12" r="9"/><path d="m8 12 3 3 5-6"/>', color="#398062")
svg("leaf", '<path d="M19 3C7 3 3 8 5 14s13 7 14-11ZM5 21l10-12"/>', color="#568B70")
svg("sun", '<circle cx="12" cy="12" r="4"/><path d="M12 1v2M12 21v2M1 12h2M21 12h2M4 4l1.5 1.5M18.5 18.5 20 20M4 20l1.5-1.5M18.5 5.5 20 4"/>', color="#BB8C51")
svg("pin", '<path d="M19 10c0 5-7 11-7 11S5 15 5 10a7 7 0 1 1 14 0Z"/><circle cx="12" cy="10" r="2.3"/>', color="#D85B28")
svg("history", '<path d="M3 11a9 9 0 1 1 2 7M3 5v6h6M12 7v5l3 2"/>')
svg("parcel", '<path d="m12 2 10 5.5v10L12 23 2 17.5v-10Z" fill="#ECD8BE" stroke="#AD8560"/><path d="m2 7.5 10 5.5 10-5.5M12 13v10M7 4.8l10 5.5v4.5l-3 1.7v-4.6L4 6.4"/>', size=32, color="#AD8560")

brand = '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" viewBox="0 0 40 40"><rect width="40" height="40" rx="12" fill="#DF6A39"/><g fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m20 8 11 6v12l-11 6-11-6V14zM9 14l11 6 11-6M20 20v12M14.5 11l11 6"/></g></svg>'
(images / "brand_mark.svg").write_text(brand, encoding="utf-8")
(root / "AppIcon/appicon.svg").write_text('<svg xmlns="http://www.w3.org/2000/svg" width="128" height="128"><rect width="128" height="128" fill="#DF6A39"/></svg>', encoding="utf-8")
mark = '<svg xmlns="http://www.w3.org/2000/svg" width="128" height="128" viewBox="0 0 40 40"><g fill="none" stroke="white" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"><path d="m20 10 9 5v10l-9 5-9-5V15zM11 15l9 5 9-5M20 20v10M15.5 12.5l9 5"/></g></svg>'
(root / "AppIcon/appiconfg.svg").write_text(mark, encoding="utf-8")
(root / "Splash/splash.svg").write_text(brand, encoding="utf-8")

(images / "parcel_journey.svg").write_text('''<svg xmlns="http://www.w3.org/2000/svg" width="440" height="360" viewBox="0 0 440 360">
<circle cx="233" cy="167" r="140" fill="#F8E4D5"/><circle cx="233" cy="167" r="112" fill="none" stroke="#EED8C8" stroke-width="1"/>
<path d="m40 260 88-47 85 47 86-48 101 56" fill="none" stroke="#EDD7C5" stroke-width="36" stroke-linejoin="round"/>
<path d="m40 260 88-47 85 47 86-48 101 56" fill="none" stroke="#FFFAF5" stroke-width="29" stroke-linejoin="round"/>
<path d="m40 260 88-47 85 47 86-48 101 56" fill="none" stroke="#D78B61" stroke-width="1.7" stroke-dasharray="5 7"/>
<ellipse cx="231" cy="244" rx="93" ry="22" fill="#D0AA8D" opacity=".16"/>
<path d="m226 94 88 49v94l-88 50-87-50v-94Z" fill="#D7AE7E" stroke="#AA7C50" stroke-width="1.3"/>
<path d="m139 143 87 50 88-50-88-49Z" fill="#F0D3A9"/>
<path d="m226 193 88-50v94l-88 50Z" fill="#C79765"/>
<path d="m183 118 89 49v37l-24 14v-38l-88-49Z" fill="#F8E5C9"/>
<path d="m139 143 87 50 88-50M226 193v94" fill="none" stroke="#A97C51" stroke-width="1.3"/>
<path d="m161 203 38 22v27l-38-22Z" fill="#FCF4E6"/><path d="m169 217 19 11M169 223l14 8" stroke="#BA9772" stroke-width="2"/>
<path d="m280 230 16-9M280 235l16-9" stroke="#9C7148" stroke-width="2"/>
<g transform="translate(321 66)"><rect width="81" height="40" rx="12" fill="white"/><circle cx="20" cy="20" r="10" fill="#EAF4E8"/><path d="m15 20 3 3 6-6" fill="none" stroke="#548A65" stroke-width="1.8" stroke-linecap="round"/><path d="M39 16h26M39 23h17" stroke="#BCB5AC" stroke-width="3" stroke-linecap="round"/></g>
<g transform="translate(49 111)"><rect width="79" height="53" rx="12" fill="white"/><path d="M16 16h33M16 24h47M16 34h25" stroke="#DFD5C9" stroke-width="3" stroke-linecap="round"/></g>
<g transform="translate(344 229)"><ellipse cx="0" cy="41" rx="14" ry="5" fill="#B78E70" opacity=".2"/><path d="M0 37s-18-18-18-30a18 18 0 1 1 36 0C18 19 0 37 0 37Z" fill="#D96535"/><circle cy="7" r="6" fill="#FFF7ED"/></g>
<g transform="translate(89 201)"><circle r="8" fill="#FFFAF5" stroke="#BEAA8E" stroke-width="2"/><circle r="3" fill="#BEAA8E"/></g>
<path d="m346 137 3-7 3 7 7 3-7 3-3 7-3-7-7-3ZM115 71l2-5 2 5 5 2-5 2-2 5-2-5-5-2Z" fill="#D6AC84"/>
<circle cx="371" cy="178" r="3" fill="#DEB690"/><circle cx="105" cy="187" r="2" fill="#DEB690"/>
</svg>''', encoding="utf-8")

(images / "route_map.svg").write_text('''<svg xmlns="http://www.w3.org/2000/svg" width="560" height="300" viewBox="0 0 560 300">
<rect width="560" height="300" fill="#EFF1EB"/>
<g fill="#E2E9DC"><path d="M0 0h115l-20 70H0zM210 0h125l-20 70H185zM435 0h125v70H416zM0 178h75l-28 122H0zM110 178h125l-30 122H80zM336 179h113l-20 121H308z"/><rect x="362" y="97" width="112" height="57" rx="8"/></g>
<path d="M129-20 49 320M350-20 266 320M492-20 408 320M-20 86h600M-20 165h600M-20 264h600" stroke="white" stroke-width="13"/>
<path d="M-10 215c75-90 134-76 174-43s54 23 110-13 96-47 163-19 91 34 140 12" stroke="#C9DEE3" stroke-width="23" fill="none"/>
<path d="M-10 215c75-90 134-76 174-43s54 23 110-13 96-47 163-19 91 34 140 12" stroke="#E1EDF0" stroke-width="15" fill="none"/>
<path d="m129-20-80 340M350-20 266 320M492-20 408 320M-20 86h600M-20 165h600M-20 264h600" stroke="#DDE2D9" stroke-width="1" fill="none"/>
<path d="M107 85h222l-19 80h136l-25 97" stroke="#E18D62" stroke-width="4" stroke-linecap="round" fill="none" stroke-dasharray="6 7"/>
<circle cx="107" cy="85" r="9" fill="white" stroke="#A6B8A5" stroke-width="3"/><circle cx="107" cy="85" r="3" fill="#789575"/>
<circle cx="310" cy="165" r="25" fill="#F5D9C7" opacity=".8"/><circle cx="310" cy="165" r="15" fill="white"/>
<g transform="translate(310 158)"><path d="M0 17s-12-12-12-20a12 12 0 1 1 24 0C12 5 0 17 0 17Z" fill="#D85B28"/><circle cy="-3" r="4" fill="white"/></g>
<g transform="translate(421 261)"><circle r="10" fill="white" stroke="#A6B8A5" stroke-width="2"/><path d="m-5 0 5-5 5 5v5H-5z" fill="none" stroke="#678A6E" stroke-width="1.5"/></g>
</svg>''', encoding="utf-8")
print("SVGs do Rastro gerados.")
