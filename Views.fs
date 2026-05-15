module Skypay.Views

// ==========================================
// VIEW LAYER (Views.fs)
// ==========================================
// This file is the "View". It takes the data from the Controller
// and turns it into HTML for the user's browser.
// I also put my custom CSS here to give it the "Cartoon" look.

open System
open Giraffe.ViewEngine
open Skypay.Models

// Professional Aviation B2B portal design.
// Palette: deep navy (#0f2044), gold (#c4a55a), clean white, slate grays.
// Typography: Inter (Google Fonts). No cartoon borders or Comic Sans.
let css = """
:root {
    --navy:       #0f2044;
    --navy-mid:   #1e3a5f;
    --gold:       #c4a55a;
    --gold-light: #f0e0b0;
    --bg:         #f0f4f8;
    --card:       #ffffff;
    --text:       #1e293b;
    --muted:      #64748b;
    --border:     #e2e8f0;
    --radius:     10px;
    --radius-lg:  16px;
    --shadow:     0 2px 12px rgba(15,32,68,0.09);
    --shadow-lg:  0 8px 32px rgba(15,32,68,0.15);
}
*, *::before, *::after { box-sizing: border-box; }
body { font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif; color: var(--text); background: var(--bg); margin: 0; padding: 0; display: flex; flex-direction: column; min-height: 100vh; font-size: 15px; line-height: 1.6; }
h1, h2, h3 { font-weight: 700; letter-spacing: -0.02em; margin-top: 0; color: var(--navy); }
.navbar { background: var(--navy); padding: 0 40px; display: flex; justify-content: space-between; align-items: center; height: 64px; position: sticky; top: 0; z-index: 1000; box-shadow: 0 2px 8px rgba(0,0,0,0.25); }
.nav-brand { font-size: 1.35rem; font-weight: 800; color: var(--gold); text-decoration: none; letter-spacing: 4px; text-transform: uppercase; }
.nav-links { display: flex; align-items: center; gap: 4px; }
.nav-links a { color: rgba(255,255,255,0.78); text-decoration: none; font-weight: 500; font-size: 0.875rem; padding: 8px 14px; border-radius: 6px; transition: all 0.15s; }
.nav-links a:hover { background: rgba(255,255,255,0.10); color: white; }
.nav-user { color: var(--gold); font-weight: 600; font-size: 0.875rem; padding: 6px 14px; border: 1px solid var(--gold); border-radius: 20px; }
.nav-badge { background: var(--gold); color: var(--navy); font-size: 0.7rem; font-weight: 700; border-radius: 10px; padding: 1px 7px; margin-left: 5px; }
.container { max-width: 1200px; margin: 0 auto; padding: 32px 24px; flex: 1; }
.header { background: linear-gradient(135deg, var(--navy) 0%, var(--navy-mid) 100%); color: white; padding: 64px 48px; border-radius: var(--radius-lg); margin-bottom: 40px; position: relative; overflow: hidden; }
.header::before { content: ''; position: absolute; inset: 0; background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.04'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E"); }
.header h1 { font-size: 2.5rem; font-weight: 800; color: white; margin-bottom: 10px; letter-spacing: -0.03em; position: relative; }
.header p { color: rgba(255,255,255,0.72); margin: 0; font-size: 1rem; position: relative; }
.header-eyebrow { font-size: 0.72rem; font-weight: 700; letter-spacing: 0.15em; text-transform: uppercase; color: var(--gold); margin: 0 0 14px; position: relative; }
.header-stats { display: flex; gap: 40px; flex-wrap: wrap; margin-top: 32px; position: relative; }
.header-stat .hs-val { font-size: 1.5rem; font-weight: 800; color: var(--gold); line-height: 1; }
.header-stat .hs-label { font-size: 0.7rem; color: rgba(255,255,255,0.55); text-transform: uppercase; letter-spacing: 0.06em; margin-top: 3px; }
.grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 24px; }
.card { background: var(--card); border-radius: var(--radius-lg); box-shadow: var(--shadow); overflow: hidden; transition: transform 0.2s, box-shadow 0.2s; display: flex; flex-direction: column; border: 1px solid var(--border); }
.card:hover { transform: translateY(-4px); box-shadow: var(--shadow-lg); }
.card-img-placeholder { height: 200px; background-color: var(--navy); background-size: cover; background-position: center; }
.card-content { padding: 20px 24px 24px; display: flex; flex-direction: column; flex: 1; }
.card-title { font-size: 1.1rem; font-weight: 700; margin-bottom: 4px; color: var(--navy); }
.card-subtitle { font-size: 1.2rem; font-weight: 700; color: var(--gold); margin-bottom: 12px; }
.card-desc { font-size: 0.875rem; color: var(--muted); line-height: 1.6; margin-bottom: 16px; flex: 1; }
.tag { display: inline-block; padding: 3px 10px; border-radius: 20px; font-size: 0.72rem; font-weight: 600; margin-bottom: 14px; letter-spacing: 0.04em; text-transform: uppercase; }
.tag.Airbus { background: #dbeafe; color: #1e40af; }
.tag.Boeing { background: #dcfce7; color: #166534; }
.tag.Embraer { background: #fef9c3; color: #854d0e; }
.tag.Bombardier { background: #fee2e2; color: #991b1b; }
.tag.COMAC { background: #f3e8ff; color: #6b21a8; }
.btn { display: inline-block; background: var(--navy); color: white; text-decoration: none; padding: 10px 20px; font-weight: 600; font-size: 0.875rem; border: none; border-radius: 8px; cursor: pointer; transition: all 0.15s; text-align: center; letter-spacing: 0.02em; font-family: 'Inter', sans-serif; }
.btn:hover { background: var(--navy-mid); transform: translateY(-1px); box-shadow: 0 4px 12px rgba(15,32,68,0.25); }
.btn:active { transform: translateY(0); box-shadow: none; }
input, select { font-family: 'Inter', sans-serif; font-size: 0.9375rem; border: 1.5px solid var(--border) !important; border-radius: 8px; background: white; transition: border-color 0.15s, box-shadow 0.15s; box-shadow: none !important; }
input:focus, select:focus { outline: none; border-color: var(--navy) !important; box-shadow: 0 0 0 3px rgba(15,32,68,0.10) !important; }
.chart-container { background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 24px; margin-bottom: 36px; box-shadow: var(--shadow); }
.payment-container { max-width: 580px; margin: 40px auto; padding: 40px; background: var(--card); border: 1px solid var(--border); box-shadow: var(--shadow-lg); border-radius: var(--radius-lg); }
.form-group { margin-bottom: 22px; position: relative; }
.form-label { display: block; margin-bottom: 7px; font-weight: 600; font-size: 0.875rem; color: var(--text); }
.form-input { width: 100%; padding: 11px 14px; border-radius: 8px; font-size: 0.9375rem; border: 1.5px solid var(--border); box-sizing: border-box; font-family: 'Inter', sans-serif; transition: border-color 0.15s, box-shadow 0.15s; }
.form-input:focus { outline: none; border-color: var(--navy); box-shadow: 0 0 0 3px rgba(15,32,68,0.10); }
.card-logos { display: flex; gap: 10px; margin-top: 8px; }
.card-logo { border: 1.5px solid var(--border); border-radius: 8px; padding: 8px 18px; cursor: pointer; font-weight: 600; font-size: 0.875rem; transition: all 0.15s; background: white; color: var(--text); }
.card-logo:hover { border-color: var(--navy); }
.card-logo.selected { border: 2px solid var(--navy); background: #eff6ff; color: var(--navy); font-weight: 700; }
.empty-state { text-align: center; padding: 60px 20px; background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); box-shadow: var(--shadow); margin: 40px 0; }
.empty-state h2 { color: var(--navy); }
.empty-state p { color: var(--muted); margin-bottom: 24px; }
.footer { background: var(--navy); color: rgba(255,255,255,0.60); text-align: center; padding: 32px; margin-top: 60px; font-size: 0.875rem; }
.footer p { margin: 5px 0; }
.footer-links a { color: var(--gold); text-decoration: none; margin: 0 10px; font-weight: 500; }
.track-container { max-width: 700px; margin: 40px auto; padding: 40px; background: var(--card); border: 1px solid var(--border); box-shadow: var(--shadow-lg); border-radius: var(--radius-lg); }
.pipeline { display: flex; justify-content: space-between; align-items: flex-start; margin: 36px 0; position: relative; }
.pipeline::before { content: ''; position: absolute; top: 20px; left: 30px; right: 30px; height: 2px; background: var(--border); z-index: 0; }
.pipeline-step { display: flex; flex-direction: column; align-items: center; gap: 8px; z-index: 1; flex: 1; }
.step-circle { width: 42px; height: 42px; border-radius: 50%; border: 2px solid var(--border); display: flex; align-items: center; justify-content: center; font-size: 0.8rem; font-weight: 700; background: white; color: var(--muted); box-shadow: var(--shadow); }
.step-done .step-circle    { background: #dcfce7; border-color: #16a34a; color: #166534; }
.step-active .step-circle  { background: var(--gold-light); border-color: var(--gold); color: var(--navy); animation: pulse 2s infinite; }
.step-pending .step-circle { background: white; border-color: var(--border); color: var(--muted); }
@keyframes pulse { 0%, 100% { box-shadow: 0 0 0 0 rgba(196,165,90,0.35); } 50% { box-shadow: 0 0 0 8px rgba(196,165,90,0); } }
.step-label { font-size: 0.64rem; font-weight: 600; text-align: center; text-transform: uppercase; letter-spacing: 0.04em; max-width: 70px; line-height: 1.3; color: var(--muted); }
.step-done .step-label   { color: #166534; }
.step-active .step-label { color: var(--gold); }
.order-info-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; margin-top: 24px; }
.order-info-cell { background: var(--bg); border: 1px solid var(--border); border-radius: 8px; padding: 12px 16px; }
.order-info-cell .oi-label { font-size: 0.7rem; font-weight: 600; color: var(--muted); text-transform: uppercase; letter-spacing: 0.06em; margin-bottom: 4px; }
.order-info-cell .oi-value { font-size: 0.9375rem; font-weight: 600; color: var(--navy); }

/* ── LEASE MODULE STYLES ─────────────────────────────────── */
.lease-hero { background: linear-gradient(135deg, #0f2044 0%, #1a3a6e 50%, #0d3320 100%); color: white; padding: 56px 48px; border-radius: var(--radius-lg); margin-bottom: 36px; position: relative; overflow: hidden; }
.lease-hero::before { content: ''; position: absolute; inset: 0; background: url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.04'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E"); }
.lease-hero h1 { font-size: 2.4rem; font-weight: 800; color: white; margin-bottom: 10px; letter-spacing: -0.03em; position: relative; }
.lease-hero p { color: rgba(255,255,255,0.72); font-size: 1.05rem; margin: 0 0 28px; position: relative; }
.lease-stats { display: flex; gap: 32px; flex-wrap: wrap; position: relative; }
.lease-stat { text-align: center; }
.lease-stat .ls-val { font-size: 1.6rem; font-weight: 800; color: var(--gold); line-height: 1; }
.lease-stat .ls-label { font-size: 0.72rem; color: rgba(255,255,255,0.6); text-transform: uppercase; letter-spacing: 0.06em; margin-top: 4px; }
.lease-card { background: var(--card); border-radius: var(--radius-lg); border: 1px solid var(--border); box-shadow: var(--shadow); overflow: hidden; transition: transform 0.2s, box-shadow 0.2s; display: flex; flex-direction: column; }
.lease-card:hover { transform: translateY(-5px); box-shadow: var(--shadow-lg); }
.lease-card-img { height: 180px; background-size: cover; background-position: center; position: relative; }
.lease-card-badge { position: absolute; top: 12px; left: 12px; background: rgba(15,32,68,0.85); color: var(--gold); font-size: 0.7rem; font-weight: 700; padding: 4px 10px; border-radius: 20px; letter-spacing: 0.05em; text-transform: uppercase; backdrop-filter: blur(4px); }
.lease-card-body { padding: 20px 24px 24px; flex: 1; display: flex; flex-direction: column; }
.lease-rate-usd { font-size: 1.4rem; font-weight: 800; color: var(--navy); line-height: 1; }
.lease-rate-lak { font-size: 0.8rem; color: var(--muted); margin-top: 2px; }
.cancel-badge { display: inline-flex; align-items: center; gap: 5px; padding: 3px 10px; border-radius: 20px; font-size: 0.7rem; font-weight: 700; letter-spacing: 0.04em; text-transform: uppercase; }
.cancel-yes { background: #dcfce7; color: #166534; }
.cancel-no  { background: #fef9c3; color: #854d0e; }
.status-pill { display: inline-block; padding: 4px 12px; border-radius: 20px; font-size: 0.72rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.05em; }
.status-Enquiry          { background: #e0f2fe; color: #0369a1; }
.status-OfferSubmitted   { background: #fef9c3; color: #854d0e; }
.status-NegotiationOpen  { background: #fed7aa; color: #9a3412; }
.status-ContractSigned   { background: #dcfce7; color: #166534; }
.status-AircraftOnGround { background: #c7d2fe; color: #3730a3; }
.status-Cancelled        { background: #fee2e2; color: #991b1b; }
.lease-form-container { max-width: 680px; margin: 0 auto; }
.lease-aircraft-summary { background: linear-gradient(135deg, var(--navy), var(--navy-mid)); color: white; border-radius: var(--radius-lg); padding: 28px 32px; margin-bottom: 32px; display: flex; gap: 24px; align-items: center; }
.lease-aircraft-summary img { width: 100px; height: 70px; object-fit: cover; border-radius: 8px; border: 2px solid rgba(255,255,255,0.2); }
.las-title { font-size: 1.4rem; font-weight: 800; color: white; margin: 0 0 4px; }
.las-mfr { color: var(--gold); font-size: 0.875rem; font-weight: 600; margin: 0 0 8px; }
.las-price { font-size: 0.82rem; color: rgba(255,255,255,0.65); }
.duration-btn { padding: 10px 20px; border: 2px solid var(--border); border-radius: 8px; background: white; cursor: pointer; font-family: 'Inter', sans-serif; font-weight: 600; font-size: 0.875rem; transition: all 0.15s; color: var(--text); }
.duration-btn:hover { border-color: var(--navy); }
.duration-btn.active { border-color: var(--gold); background: var(--gold-light); color: var(--navy); }
.price-preview-box { background: linear-gradient(135deg, #f0fdf4, #dcfce7); border: 2px solid #16a34a; border-radius: 12px; padding: 20px 24px; margin: 20px 0; animation: slideUp 0.3s ease; }
@keyframes slideUp { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
.lease-activity-row { display: flex; gap: 16px; align-items: center; padding: 16px; background: white; border: 1px solid var(--border); border-radius: 10px; transition: box-shadow 0.15s; }
.lease-activity-row:hover { box-shadow: var(--shadow); }
.lar-flag { font-size: 1.5rem; width: 40px; text-align: center; flex-shrink: 0; }
.lar-body { flex: 1; }
.lar-airline { font-weight: 700; font-size: 0.95rem; color: var(--navy); margin: 0 0 2px; }
.lar-detail { font-size: 0.8rem; color: var(--muted); margin: 0; }
/* ── SCROLL ANIMATIONS ───────────────────────────────────── */
@keyframes fadeInUp { from { opacity: 0; transform: translateY(28px); } to { opacity: 1; transform: translateY(0); } }
.anim { opacity: 0; }
.anim.visible { animation: fadeInUp 0.55s ease forwards; }
/* ── FOOTER COLUMNS ──────────────────────────────────────── */
.footer-inner { max-width: 1200px; margin: 0 auto; }
.footer-cols { display: grid; grid-template-columns: repeat(3, 1fr); gap: 32px; margin-bottom: 28px; text-align: left; }
.footer-col h4 { color: white; font-size: 0.75rem; font-weight: 700; letter-spacing: 0.1em; text-transform: uppercase; margin: 0 0 12px; }
.footer-col p { color: rgba(255,255,255,0.48); font-size: 0.82rem; margin: 5px 0; }
.footer-col a { color: rgba(255,255,255,0.55); font-size: 0.82rem; margin: 5px 0; display: block; text-decoration: none; transition: color 0.15s; }
.footer-col a:hover { color: var(--gold); }
.footer-divider { border: none; border-top: 1px solid rgba(255,255,255,0.10); margin: 0 0 20px; }
/* ── TRACK HOW-IT-WORKS ──────────────────────────────────── */
.hiw-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin-top: 40px; }
.hiw-step { background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 24px; text-align: center; box-shadow: var(--shadow); }
.hiw-num { width: 42px; height: 42px; border-radius: 50%; background: var(--navy); color: var(--gold); font-weight: 800; font-size: 1rem; display: flex; align-items: center; justify-content: center; margin: 0 auto 12px; }
.hiw-title { font-weight: 700; font-size: 0.9rem; color: var(--navy); margin: 0 0 6px; }
.hiw-desc { font-size: 0.78rem; color: var(--muted); line-height: 1.55; margin: 0; }
/* ── HAMBURGER / MOBILE NAV ──────────────────────────────── */
.hamburger { display: none; flex-direction: column; gap: 5px; background: none; border: none; cursor: pointer; padding: 4px; }
.hamburger span { display: block; width: 24px; height: 2px; background: white; border-radius: 2px; transition: all 0.25s; }
.hamburger.open span:nth-child(1) { transform: translateY(7px) rotate(45deg); }
.hamburger.open span:nth-child(2) { opacity: 0; }
.hamburger.open span:nth-child(3) { transform: translateY(-7px) rotate(-45deg); }
/* ── CONTACT PAGE ────────────────────────────────────────── */
.contact-hero { background: linear-gradient(135deg, var(--navy) 0%, #1a3a6e 100%); color: white; padding: 56px 48px; border-radius: var(--radius-lg); margin-bottom: 40px; text-align: center; }
.contact-hero h1 { color: white; font-size: 2.4rem; margin-bottom: 10px; }
.contact-hero p { color: rgba(255,255,255,0.75); font-size: 1.05rem; margin: 0; }
.contact-grid { display: grid; grid-template-columns: 1fr 1.6fr; gap: 32px; align-items: start; }
.contact-info-card { background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 28px; box-shadow: var(--shadow); }
.contact-info-item { display: flex; gap: 14px; align-items: flex-start; margin-bottom: 22px; }
.contact-info-item:last-child { margin-bottom: 0; }
.ci-icon { width: 40px; height: 40px; border-radius: 10px; background: linear-gradient(135deg, var(--navy), #1a3a6e); display: flex; align-items: center; justify-content: center; flex-shrink: 0; font-size: 1.1rem; }
.ci-label { font-size: 0.72rem; font-weight: 700; text-transform: uppercase; letter-spacing: 0.07em; color: var(--muted); margin: 0 0 3px; }
.ci-value { font-size: 0.9rem; font-weight: 600; color: var(--navy); margin: 0; }
.contact-form-card { background: var(--card); border: 1px solid var(--border); border-radius: var(--radius-lg); padding: 32px; box-shadow: var(--shadow); }
.contact-success { text-align: center; padding: 48px 24px; }
.contact-success-icon { width: 72px; height: 72px; border-radius: 50%; background: linear-gradient(135deg, #dcfce7, #bbf7d0); border: 3px solid #16a34a; display: flex; align-items: center; justify-content: center; margin: 0 auto 20px; font-size: 2rem; }
textarea.form-input { resize: vertical; min-height: 120px; line-height: 1.6; }
/* ── RESPONSIVE MEDIA QUERIES ────────────────────────────── */
@media (max-width: 768px) {
  .hamburger { display: flex; }
  .nav-links { display: none; flex-direction: column; position: absolute; top: 64px; left: 0; right: 0; background: var(--navy); padding: 16px 20px 20px; gap: 2px; box-shadow: 0 8px 24px rgba(0,0,0,0.3); z-index: 999; }
  .nav-links.open { display: flex; }
  .nav-links a { padding: 11px 14px; border-radius: 8px; width: 100%; font-size: 0.95rem; }
  .navbar { padding: 0 20px; position: relative; }
  .container { padding: 20px 16px; }
  .header { padding: 36px 24px; }
  .header h1 { font-size: 1.7rem; }
  .header-stats { gap: 20px; }
  .grid { grid-template-columns: 1fr; }
  .payment-container { margin: 20px 12px; padding: 24px 18px; }
  .track-container { margin: 20px 12px; padding: 24px 18px; }
  .lease-form-container { margin: 0 8px; }
  .lease-hero { padding: 36px 20px; }
  .lease-hero h1 { font-size: 1.8rem; }
  .lease-stats { gap: 16px; }
  .lease-aircraft-summary { flex-direction: column; padding: 20px; gap: 16px; }
  .footer-cols { grid-template-columns: 1fr; gap: 20px; }
  .hiw-grid { grid-template-columns: 1fr; }
  .order-info-grid { grid-template-columns: 1fr; }
  .order-info-cell[style*='span 2'] { grid-column: span 1 !important; }
  .pipeline { flex-direction: column; gap: 12px; align-items: flex-start; }
  .pipeline::before { display: none; }
  .pipeline-step { flex-direction: row; gap: 12px; }
  .contact-grid { grid-template-columns: 1fr; }
  .contact-hero { padding: 36px 20px; }
  .contact-hero h1 { font-size: 1.8rem; }
  div[style*='grid-template-columns: 1fr 1fr'] { display: block !important; }
  div[style*='grid-template-columns:1fr 1fr'] { display: block !important; }
  div[style*='display: grid'] { gap: 0 !important; }
  .lease-hero .lease-stats { flex-wrap: wrap; }
}
"""

let layout (title: string) (cartCount: int) (username: string option) (content: XmlNode list) =
    html [] [
        head [] [
            meta [ _charset "UTF-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            meta [ _name "description"; _content "Skypay — B2B aircraft sales and leasing marketplace for Southeast Asian operators. Buy or lease commercial aircraft with real-time USD and LAK pricing." ]
            tag "title" [] [ str title ]
            link [ _rel "stylesheet"; _href "https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap" ]
            script [ _src "https://cdn.jsdelivr.net/npm/chart.js" ] []
            style [] [ str css ]
        ]
        body [] (
            [
                nav [ _class "navbar" ] [
                    a [ _href "/"; _class "nav-brand" ] [ str "SKYPAY" ]
                    button [ _class "hamburger"; _id "hamburger"; attr "aria-label" "Toggle menu"; attr "onclick" "document.getElementById('nav-links').classList.toggle('open');this.classList.toggle('open');" ] [
                        span [] []; span [] []; span [] []
                    ]
                    div [ _class "nav-links"; _id "nav-links" ] [
                        a [ _href "/" ] [ str "Home" ]
                        a [ _href "/lease"; _style "color:var(--gold);font-weight:600;" ] [ str "Lease Aircraft" ]
                        a [ _href "/track" ] [ str "Track Order" ]
                        a [ _href "/contact" ] [ str "Contact Agency" ]
                        a [ _href "/cart"; _style "position:relative;" ] [
                            str "Cart"
                            if cartCount > 0 then
                                span [ _style "position:absolute;top:-8px;right:-10px;background:var(--gold);color:var(--navy);border-radius:50%;width:20px;height:20px;font-size:0.7rem;font-weight:900;display:flex;align-items:center;justify-content:center;" ] [ str (string cartCount) ]
                        ]
                        match username with
                        | Some name ->
                            span [ _style "font-weight:600;padding:6px 14px;border:1px solid var(--gold);border-radius:20px;color:var(--gold);" ] [ str name ]
                            a [ _href "/logout"; _style "color:#dc2626;font-weight:bold;padding:8px 12px;border:2px solid #dc2626;border-radius:8px;margin-left:8px;text-decoration:none;transition:all 0.2s;" ] [ str "Logout" ]
                        | None ->
                            a [ _href "/login"    ] [ str "Login" ]
                            a [ _href "/register"; _style "background:var(--gold);color:var(--navy);border-radius:6px;font-weight:600;" ] [ str "Register" ]
                    ]
                ]
                div [ _class "container" ] content
                footer [ _class "footer" ] [
                    div [ _class "footer-inner" ] [
                        div [ _class "footer-cols" ] [
                            div [ _class "footer-col" ] [
                                h4 [] [ str "Skypay" ]
                                p [] [ str "B2B aircraft marketplace for" ]
                                p [] [ str "Southeast Asian operators." ]
                                p [] [ str "Est. 2026 · Vientiane, Laos" ]
                            ]
                            div [ _class "footer-col" ] [
                                h4 [] [ str "Services" ]
                                a [ _href "/" ] [ str "Browse Aircraft" ]
                                a [ _href "/lease" ] [ str "Lease Aircraft" ]
                                a [ _href "/track" ] [ str "Track Order" ]
                                a [ _href "/contact" ] [ str "Contact Agency" ]
                                a [ _href "/cart" ] [ str "Shopping Cart" ]
                            ]
                            div [ _class "footer-col" ] [
                                h4 [] [ str "Markets" ]
                                p [] [ str "Laos · Thailand · Vietnam" ]
                                p [] [ str "Cambodia · Myanmar" ]
                                p [] [ str "USD / LAK dual currency" ]
                            ]
                        ]
                        hr [ _class "footer-divider" ]
                        p [] [ str "© 2026 Skypay Aircraft Shop. All flights reserved. Created by Silichanh SIPHANH." ]
                        div [ _class "footer-links" ] [
                            a [ _href "#" ] [ str "About Us" ]
                            a [ _href "#" ] [ str "Support" ]
                            a [ _href "#" ] [ str "Terms of Service" ]
                        ]
                    ]
                ]
            ]
        )
    ]

// Moved template out to avoid parser confusion
let chartScriptTemplate = """
    const ctx = document.getElementById('salesChart');
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['%s'],
            datasets: [{
                label: '# of Aircraft Sold',
                data: [%s],
                backgroundColor: 'rgba(56, 189, 248, 0.6)',
                borderColor: 'rgba(56, 189, 248, 1)',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    labels: { color: '#94a3b8' }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    grid: { color: 'rgba(255, 255, 255, 0.1)' },
                    ticks: { color: '#94a3b8' }
                },
                x: {
                    grid: { display: false },
                    ticks: { color: '#94a3b8' }
                }
            }
        }
    });
"""

let dashboardView (aircrafts: Aircraft list) (topStats: Aircraft list) (searchQuery: string) (brandFilter: string) (cartCount: int) (username: string option) =
    let searchVal = if isNull searchQuery then "" else searchQuery
    let brandVal = if isNull brandFilter then "" else brandFilter
    let manufacturers = [ "All"; "Airbus"; "Boeing"; "Embraer"; "Bombardier"; "COMAC" ]
    // Generate JS arrays manually to avoid sprintf %s conflicts
    let jsLabels = "[" + (topStats |> List.map (fun a -> sprintf "'%s'" a.Model) |> String.concat ",") + "]"
    let jsData = "[" + (topStats |> List.map (fun a -> string a.SalesCount) |> String.concat ",") + "]"

    // JS for the chart
    let chartScript = 
        String.concat "\n" [
            "const ctx = document.getElementById('salesChart');"
            "new Chart(ctx, {"
            "    type: 'bar',"
            "    data: {"
            "        labels: " + jsLabels + ","
            "        datasets: [{"
            "            label: '# of Aircraft Sold (Singapore Airshow 2026)',"
            "            data: " + jsData + ","
            "            backgroundColor: 'rgba(56, 189, 248, 0.6)',"
            "            borderColor: 'rgba(56, 189, 248, 1)',"
            "            borderWidth: 1,"
            "            borderRadius: 4"
            "        }]"
            "    },"
            "    options: {"
            "        responsive: true,"
            "        maintainAspectRatio: false,"
            "        plugins: {"
            "            legend: {"
            "                labels: { color: '#94a3b8' }"
            "            }"
            "        },"
            "        scales: {"
            "            y: {"
            "                beginAtZero: true,"
            "                grid: { color: 'rgba(255, 255, 255, 0.1)' },"
            "                ticks: { color: '#94a3b8' }"
            "            },"
            "            x: {"
            "                grid: { display: false },"
            "                ticks: { color: '#94a3b8' }"
            "            }"
            "        }"
            "    }"
            "});"
        ]

    layout "Skypay — Aircraft Marketplace" cartCount username [
        div [ _class "header" ] [
            div [ _style "position:relative;z-index:1;" ] [
                p [ _class "header-eyebrow" ] [ str "B2B Aviation Marketplace" ]
                h1 [] [ str "Global Aircraft Shop" ]
                p [] [ str (sprintf "%d commercial aircraft available — B2B sales & leasing for Southeast Asian operators — Real-time USD & LAK pricing" aircrafts.Length) ]
                div [ _class "header-stats" ] [
                    div [ _class "header-stat" ] [
                        div [ _class "hs-val" ] [ str (string aircrafts.Length) ]
                        div [ _class "hs-label" ] [ str "Aircraft Listed" ]
                    ]
                    div [ _class "header-stat" ] [
                        div [ _class "hs-val" ] [ str "5" ]
                        div [ _class "hs-label" ] [ str "Manufacturers" ]
                    ]
                    div [ _class "header-stat" ] [
                        div [ _class "hs-val" ] [ str "$250B+" ]
                        div [ _class "hs-label" ] [ str "Market Value" ]
                    ]
                    div [ _class "header-stat" ] [
                        div [ _class "hs-val" ] [ str "LAK" ]
                        div [ _class "hs-label" ] [ str "Local Currency" ]
                    ]
                ]
            ]
        ]

        // Search & Filter Section
        div [ _style "background: white; padding: 20px; border-radius: 12px; margin-bottom: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.05);" ] [
            form [ _action "/"; _method "get"; _style "display: flex; gap: 15px; align-items: end; flex-wrap: wrap;" ] [
                // Search Input
                div [ _style "flex-grow: 1;" ] [
                    label [ _for "q"; _style "display: block; margin-bottom: 5px; font-weight: 500; font-size: 0.9em; color: #64748b;" ] [ str "Search Model" ]
                    input [ 
                        _type "text"
                        _name "q"
                        _id "q"
                        _placeholder "e.g. 737, A320..."
                        _value searchVal
                        _style "width: 100%; padding: 10px 15px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px;" 
                    ]
                ]

                // Brand Filter
                div [ _style "min-width: 200px;" ] [
                    label [ _for "brand"; _style "display: block; margin-bottom: 5px; font-weight: 500; font-size: 0.9em; color: #64748b;" ] [ str "Manufacturer" ]
                    select [ 
                        _name "brand"
                        _id "brand"
                        _style "width: 100%; padding: 10px 15px; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 16px; background-color: white;" 
                    ] [
                        for m in manufacturers do
                            let label = if m = "All" then "All Manufacturers" else m
                            let value = if m = "All" then "" else m
                            let isSelected = if value = brandVal then [ _selected ] else []
                            yield option ([ _value value ] @ isSelected) [ str label ]
                    ]
                ]

                // Submit Button
                button [ _type "submit"; _style "padding: 10px 25px; background-color: #0f172a; color: white; border: none; border-radius: 8px; font-weight: 600; cursor: pointer; height: 42px;" ] [ 
                    str "Filter" 
                ]
                
                // Clear Button (only show if filtering)
                if searchVal <> "" || brandVal <> "" then
                    a [ _href "/"; _style "padding: 10px 15px; color: #64748b; text-decoration: none; font-weight: 500; display: flex; align-items: center;" ] [ str "Clear" ]
            ]
        ]

        div [ _class "chart-container" ] [
            h2 [] [ str "Most Popular Models (Singapore Airshow 2026)" ]
            div [ _style "height: 300px; position: relative;" ] [
                canvas [ _id "salesChart" ] []
            ]
        ]

        div [ _style "display:flex;align-items:center;gap:14px;margin-bottom:4px;" ] [
            h2 [ _style "margin:0;" ] [ str (sprintf "Available Aircraft") ]
            span [ _style "background:var(--navy);color:var(--gold);font-size:0.75rem;font-weight:700;padding:4px 12px;border-radius:20px;" ] [ str (sprintf "%d models" aircrafts.Length) ]
        ]

        if aircrafts.Length = 0 then
            div [ _class "empty-state" ] [
                div [ _class "emoji" ] [ str "No results" ]
                h2 [] [ str "No Aircraft Found!" ]
                p [] [ str "Looks like we don't have what you're looking for." ]
                a [ _href "/"; _class "btn" ] [ str "Clear Filters" ]
            ]
        else
            div [ _class "grid" ] (
                aircrafts |> List.map (fun item ->
                    // Simplified description logic
                    let descText = 
                        if item.Description.Length > 80 then 
                            item.Description.Substring(0, 80) + "..." 
                        else 
                            item.Description
                    
                    let bgStyle = sprintf "background-image: url('%s');" item.ImageUrl
                    // Strip <USD> unit for display formatting (decimal/1m<USD> removes the unit)
                    let priceText = sprintf "$%s USD" ((item.Price / 1m<USD>).ToString("N0"))
                    let linkUrl = sprintf "/checkout/%s" (item.Id.ToString())

                    div [ _class "card anim" ] [
                        // Placeholder image div
                        div [ _class "card-img-placeholder"; _style bgStyle ] []
                        div [ _class "card-content" ] [
                            h3 [ _class "card-title" ] [ str item.Model ]
                            div [ _class "card-subtitle" ] [ str priceText ]
                            p [ _class "card-desc" ] [ str descText ]
                            div [] [
                    span [ _class (sprintf "tag %s" item.Manufacturer) ] [ str item.Manufacturer ]
                            ]
                            div [ _style "display:flex;gap:10px;margin-top:auto;flex-wrap:wrap;" ] [
                                a [ _href linkUrl; _class "btn"; _style "flex:1;text-align:center;" ] [ str "Buy Now" ]
                                tag "form" [ attr "action" (sprintf "/cart/add/%s" (item.Id.ToString())); attr "method" "POST"; attr "style" "flex:1;" ] [
                                    button [ _type "submit"; _class "btn"; _style "width:100%;background:#3b82f6;color:white;" ] [ str "Add to Cart" ]
                                ]
                            ]
                        ]
                    ]
                )
            )

        script [] [ rawText chartScript ]
        script [] [ rawText """
            const animEls = document.querySelectorAll('.anim');
            const observer = new IntersectionObserver((entries) => {
                entries.forEach((e, i) => {
                    if (e.isIntersecting) {
                        setTimeout(() => e.target.classList.add('visible'), i * 80);
                        observer.unobserve(e.target);
                    }
                });
            }, { threshold: 0.1 });
            animEls.forEach(el => observer.observe(el));
        """ ]
    ]

let paymentView (aircraft: Aircraft) (cartCount: int) (username: string option) =
    let priceInfo = sprintf "$%s" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let actionUrl = sprintf "/process-payment/%O" aircraft.Id
    // Build JS by concatenation to avoid sprintf %% conflicts with % in JS
    let priceJs   = string (int (aircraft.Price / 1m<USD>))
    let jsScript  =
        "const applyDiscount = rate => price => price * (1 - rate);\n" +
        "const vipDiscount   = applyDiscount(0.20);\n" +
        "const staffDiscount = applyDiscount(0.50);\n" +
        "const getDiscountFn = code => {\n" +
        "  switch (code.trim().toUpperCase()) {\n" +
        "    case 'VIP20':   return { fn: vipDiscount,   label: 'VIP20 - 20% Off!' };\n" +
        "    case 'STAFF50': return { fn: staffDiscount, label: 'STAFF50 - 50% Off!' };\n" +
        "    default:        return null;\n" +
        "  }\n" +
        "};\n" +
        "const originalPrice = " + priceJs + ";\n" +
        "function formatPrice(n) { return '$' + Math.round(n).toLocaleString('en-US') + ' USD'; }\n" +
        "function validatePromo(code) {\n" +
        "  const badge   = document.getElementById('promo-badge');\n" +
        "  const hint    = document.getElementById('promo-hint');\n" +
        "  const preview = document.getElementById('price-preview');\n" +
        "  if (code.trim() === '') {\n" +
        "    badge.innerHTML = '';\n" +
        "    if (hint) badge.appendChild(hint);\n" +
        "    preview.style.display = 'none'; return;\n" +
        "  }\n" +
        "  const result = getDiscountFn(code);\n" +
        "  if (result) {\n" +
        "    const finalPrice = result.fn(originalPrice);\n" +
        "    const saved      = originalPrice - finalPrice;\n" +
        "    badge.innerHTML = '<div style=\"display:flex;align-items:center;gap:10px;padding:10px 14px;background:#dcfce7;border:2px solid #16a34a;border-radius:8px;box-shadow:2px 2px 0px black;animation:popIn 0.2s ease;\"><span style=\"font-size:1.3rem;\">&#9989;</span><span style=\"font-weight:900;color:#15803d;\">' + result.label + '</span></div>';\n" +
        "    document.getElementById('original-price').textContent  = formatPrice(originalPrice);\n" +
        "    document.getElementById('discounted-price').textContent = formatPrice(finalPrice);\n" +
        "    document.getElementById('saved-amount').textContent     = formatPrice(saved);\n" +
        "    preview.style.display = 'block';\n" +
        "  } else {\n" +
        "    badge.innerHTML = '<div style=\"display:flex;align-items:center;gap:10px;padding:10px 14px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;animation:popIn 0.2s ease;\"><span style=\"font-size:1.3rem;\">&#10060;</span><span style=\"font-weight:900;color:#dc2626;\">Invalid promo code</span></div>';\n" +
        "    preview.style.display = 'none';\n" +
        "  }\n" +
        "}\n" +
        "function selectCard(el) {\n" +
        "  document.querySelectorAll('.card-logo').forEach(e => e.classList.remove('selected'));\n" +
        "  el.classList.add('selected');\n" +
        "  document.getElementById('cardType').value = el.innerText;\n" +
        "}\n" +
        "const animStyle = document.createElement('style');\n" +
        "animStyle.textContent = '@keyframes popIn { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }';\n" +
        "document.head.appendChild(animStyle);\n"

    layout "Checkout" cartCount username [
        div [ _class "payment-container" ] [
            div [ _style "text-align: center; margin-bottom: 30px;" ] [
                h2 [] [ str "Checkout" ]
                p [] [ str (sprintf "Buying: %s" aircraft.Model) ]
                h3 [] [ str priceInfo ]
            ]
            
            tag "form" [ attr "action" actionUrl; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label" ] [ str "Payment Method" ]
                    div [ _class "card-logos" ] [
                        div [ _class "card-logo selected"; attr "onclick" "selectCard(this)" ] [ str "VISA" ]
                        div [ _class "card-logo"; attr "onclick" "selectCard(this)" ] [ str "MasterCard" ]
                    ]
                    input [ _type "hidden"; _name "cardType"; _id "cardType"; _value "VISA" ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "cardNumber" ] [ str "Card Number" ]
                    input [ _type "text"; _name "cardNumber"; _id "cardNumber"; _class "form-input"; _placeholder "0000 0000 0000 0000"; _required ]
                ]
                div [ _style "display: grid; grid-template-columns: 1fr 1fr; gap: 20px;" ] [
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "expiry" ] [ str "Expiry Date" ]
                        input [ _type "text"; _name "expiry"; _id "expiry"; _class "form-input"; _placeholder "MM/YY"; _required ]
                    ]
                    div [ _class "form-group" ] [
                        label [ _class "form-label"; attr "for" "cvc" ] [ str "CVC" ]
                        input [ _type "text"; _name "cvc"; _id "cvc"; _class "form-input"; _placeholder "123"; _required ]
                    ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "airport" ] [ str "Delivery Airport Address" ]
                    input [ _type "text"; _name "airport"; _id "airport"; _class "form-input"; _placeholder "Full Address of Airport or ICAO Code"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "country" ] [ str "Country (Detailed)" ]
                    input [ _type "text"; _name "country"; _id "country"; _class "form-input"; _placeholder "Singapore, Cambodia, etc."; _required ]
                ]

                // --- LIVE PROMO CODE VALIDATOR ---
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "promoCode" ] [ str "Promo Code (Optional)" ]
                    input [
                        _type "text"; _name "promoCode"; _id "promoCode"; _class "form-input"
                        _placeholder "e.g. VIP20 or STAFF50"
                        attr "oninput" "validatePromo(this.value)"
                        attr "autocomplete" "off"
                        attr "style" "text-transform:uppercase;"
                    ]
                    div [ _id "promo-badge"; _style "margin-top: 10px; min-height: 40px;" ] [
                        p [ _id "promo-hint"; _style "font-size: 0.82rem; color: #64748b; margin: 0;" ] [
                            str "Try: "; strong [] [ str "VIP20" ]
                            str " (20% off)  |  "; strong [] [ str "STAFF50" ]
                            str " (50% off)"
                        ]
                    ]
                ]

                // Dynamic price preview box (shown when valid code entered)
                div [ _id "price-preview"; _style "display:none; margin-bottom: 20px; padding: 16px; background:#f0fdf4; border: 3px solid #16a34a; border-radius: 12px; box-shadow: 3px 3px 0px black; text-align:center;" ] [
                    p [ _style "margin: 0; font-size: 0.85rem; color: #64748b; text-decoration: line-through;" ] [
                        str "Original: "; span [ _id "original-price" ] []
                    ]
                    p [ _style "margin: 5px 0 0 0; font-size: 1.5rem; font-weight: 900; color: #16a34a;" ] [
                        str "You pay: "; span [ _id "discounted-price" ] []
                    ]
                    p [ _style "margin: 4px 0 0 0; font-size: 0.85rem; color: #475569;" ] [
                        str "You save: "; span [ _id "saved-amount" ] []
                    ]
                ]

                button [ _type "submit"; _class "btn"; _style "margin-top: 10px; width: 100%;" ] [ str "Complete Purchase" ]
            ]
            p [ _style "text-align: center; margin-top: 20px; font-size: 0.8rem; color: #64748b;" ] [ str "Secure 256-bit SSL Encrypted Transaction" ]
        ]
        script [] [ rawText jsScript ]
    ]

// Updated to accept finalPrice (after lambda discount) and a discountMsg
let successView (aircraft: Aircraft) (finalPrice: decimal<USD>) (discountMsg: string) (cartCount: int) (username: string option) =
    let originalPriceText = sprintf "$%s USD" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let finalPriceText    = sprintf "$%s USD" ((finalPrice / 1m<USD>).ToString("N0"))
    let hadDiscount       = discountMsg <> "" && finalPrice < aircraft.Price

    layout "Order Placed" cartCount username [
        div [ _class "container"; _style "text-align: center; padding-top: 50px;" ] [
            
            h1 [] [ str "Order Confirmed!" ]
            p [ _style "font-size: 1.2rem;" ] [ str (sprintf "We have received your order for the %s." aircraft.Model) ]

            // Discount Summary Box (only shows if a promo code was used)
            if hadDiscount then
                div [ _style "margin: 25px auto; max-width: 400px; background: #fef08a; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-weight: 900; font-size: 1.1rem; margin: 0 0 10px 0;" ] [ str discountMsg ]
                    p [ _style "margin: 5px 0; text-decoration: line-through; color: #64748b;" ] [ str (sprintf "Original Price: %s" originalPriceText) ]
                    p [ _style "margin: 5px 0; font-size: 1.5rem; font-weight: 900; color: #16a34a;" ] [ str (sprintf "You Paid: %s" finalPriceText) ]
                    p [ _style "margin: 10px 0 0 0; font-size: 0.85rem; color: #475569;" ] [ str (sprintf "You saved: $%s USD" ((aircraft.Price - finalPrice) / 1m<USD> |> fun x -> x.ToString("N0"))) ]
                ]
            else
                div [ _style "margin: 25px auto; max-width: 400px; background: white; border: 3px solid black; border-radius: 12px; padding: 20px; box-shadow: 4px 4px 0px black;" ] [
                    p [ _style "font-size: 1.4rem; font-weight: 900; margin: 0;" ] [ str (sprintf "Total Paid: %s" finalPriceText) ]
                ]

            p [ _style "color: #64748b; margin-top: 15px;" ] [ str "We will contact you shortly about delivery." ]
            // Tracking hint box
            div [ _style "margin: 20px auto; max-width: 420px; background: #eff6ff; border: 3px solid black; border-radius: 12px; padding: 16px 20px; box-shadow: 4px 4px 0px black; text-align:left;" ] [
                p [ _style "margin:0 0 6px 0; font-weight:900; font-size:1rem;" ] [ str "Track Your Delivery" ]
                p [ _style "margin:0 0 10px 0; font-size:0.85rem; color:#475569;" ] [ str "Use one of these demo order numbers on the Track page:" ]
                p [ _style "margin:0; font-size:0.85rem; font-weight:700;" ] [
                    str "SKY-2026-001  •  SKY-2026-002  •  SKY-2026-003"
                ]
            ]
            div [ _style "margin-top:20px; display:flex; gap:12px; justify-content:center; flex-wrap:wrap;" ] [
                a [ _href "/"; _class "btn" ] [ str "← Back to Shop" ]
                a [ _href "/track"; _class "btn"; _style "background:var(--accent);color:white;" ] [ str "Track Order" ]
            ]
        ]
    ]

// ==========================================
// ORDER TRACKING VIEW
// ==========================================
// This view uses pattern matching on the DeliveryStatus DU.
// The compiler guarantees we handle ALL 5 cases — no bugs from missed branches.
let orderTrackView (query: string) (result: OrderRecord option) (cartCount: int) (username: string option) =
    let queryDisplay = if isNull query then "" else query.ToUpper()

    // The 5 pipeline steps: (icon, label)
    let steps = [
        ("1", "Order Confirmed")
        ("2", "In Production")
        ("3", "Ready for Delivery")
        ("4", "In Transit")
        ("5", "Delivered")
    ]

    // Pattern match on DeliveryStatus to get the active step index
    let activeStep =
        match result with
        | Some { Status = OrderConfirmed }   -> 0
        | Some { Status = InProduction }     -> 1
        | Some { Status = ReadyForDelivery } -> 2
        | Some { Status = InTransit }        -> 3
        | Some { Status = Delivered }        -> 4
        | None                               -> -1

    // Pattern match to render the correct status badge
    let statusBadgeHtml =
        match result with
        | Some { Status = s } ->
            let text, color =
                match s with
                | OrderConfirmed   -> "Order Confirmed",      "#93c5fd"
                | InProduction     -> "In Production",       "#fde047"
                | ReadyForDelivery -> "Ready for Delivery",  "#86efac"
                | InTransit        -> "In Transit",          "#fbbf24"
                | Delivered        -> "Delivered",           "#4ade80"
            [ span [ _style (sprintf "display:inline-block;padding:6px 18px;background:%s;border:2px solid black;border-radius:20px;font-weight:900;box-shadow:2px 2px 0px black;font-size:1rem;" color) ] [ str text ] ]
        | None -> []

    // Build the pipeline nodes from the steps list
    let buildPipeline () =
        steps |> List.mapi (fun i (icon, label) ->
            let cls =
                if   i < activeStep then "pipeline-step step-done"
                elif i = activeStep then "pipeline-step step-active"
                else                     "pipeline-step step-pending"
            let circleIcon = if i < activeStep then "✓" else icon
            div [ _class cls ] [
                div [ _class "step-circle" ] [ str circleIcon ]
                span [ _class "step-label" ] [ str label ]
            ]
        )

    // Build the result HTML section
    let resultSection =
        match result with
        | None when queryDisplay <> "" ->
            [ div [ _style "text-align:center;padding:30px;background:#fee2e2;border:3px solid #dc2626;border-radius:12px;box-shadow:4px 4px 0px black;" ] [
                h2 [ _style "color:#dc2626;" ] [ str "Order Not Found" ]
                p [] [ str (sprintf "No order found matching \"%s\". Please double-check your order number." queryDisplay) ]
              ] ]
        | Some order ->
            [
                div [ _style "text-align:center;margin-bottom:18px;" ] statusBadgeHtml
                div [ _style "background:var(--primary);border:3px solid black;border-radius:12px;padding:18px 20px;text-align:center;box-shadow:4px 4px 0px black;margin-bottom:25px;" ] [
                    h2 [ _style "margin:0 0 4px 0;" ] [ str order.AircraftModel ]
                    p  [ _style "margin:0;font-weight:bold;" ] [ str (sprintf "Manufactured by %s" order.Manufacturer) ]
                ]
                div [ _class "pipeline" ] (buildPipeline ())
                div [ _class "order-info-grid" ] [
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Order Number" ]
                        div [ _class "oi-value" ] [ str order.OrderNumber ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Order Date" ]
                        div [ _class "oi-value" ] [ str order.OrderDate ]
                    ]
                    div [ _class "order-info-cell"; _style "grid-column:span 2;" ] [
                        div [ _class "oi-label" ] [ str "Delivery Destination" ]
                        div [ _class "oi-value" ] [ str order.Destination ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Est. Delivery" ]
                        div [ _class "oi-value" ] [ str order.EstimatedDate ]
                    ]
                    div [ _class "order-info-cell" ] [
                        div [ _class "oi-label" ] [ str "Manufacturer" ]
                        div [ _class "oi-value" ] [ str order.Manufacturer ]
                    ]
                ]
                div [ _style "margin-top:28px;display:flex;gap:12px;justify-content:center;flex-wrap:wrap;" ] [
                    a [ _href "/"; _class "btn" ] [ str "← Back to Shop" ]
                    a [ _href "/track"; _class "btn"; _style "background:var(--accent);color:white;" ] [ str "Track Another" ]
                ]
            ]
        | _ -> []

    // Static top section — always shown
    let topSection = [
        div [ _style "text-align:center;margin-bottom:28px;" ] [
            h1 [] [ str "Track Your Order" ]
            p [ _style "color:#64748b;" ] [ str "Enter your Skypay order number to check your aircraft delivery status." ]
        ]
        form [ _action "/track"; _method "get"; _style "display:flex;gap:10px;margin-bottom:14px;" ] [
            input [
                _type "text"; _name "order"; _id "orderInput"
                _placeholder "e.g. SKY-2026-001"
                _value queryDisplay
                _style "flex:1;padding:12px 16px;border-radius:8px;font-size:16px;text-transform:uppercase;"
            ]
            button [ _type "submit"; _class "btn"; _style "white-space:nowrap;" ] [ str "Search" ]
        ]
        p [ _style "font-size:0.82rem;color:#64748b;margin-bottom:22px;" ] [
            str "Demo orders: "
            strong [] [ str "SKY-2026-001" ]; str "  •  "
            strong [] [ str "SKY-2026-002" ]; str "  •  "
            strong [] [ str "SKY-2026-003" ]; str "  •  "
            strong [] [ str "SKY-2026-004" ]; str "  •  "
            strong [] [ str "SKY-2026-005" ]
        ]
    ]

    layout "Track Your Order | Skypay" cartCount username [
        div [ _class "track-container" ] (topSection @ resultSection)
        div [ _style "margin-top:48px;" ] [
            h2 [ _style "text-align:center;margin-bottom:8px;" ] [ str "How Aircraft Delivery Works" ]
            p [ _style "text-align:center;color:var(--muted);margin:0 0 28px;font-size:0.9rem;" ] [ str "From order confirmation to your runway — here's what to expect." ]
            div [ _class "hiw-grid" ] [
                div [ _class "hiw-step" ] [
                    div [ _class "hiw-num" ] [ str "1" ]
                    p [ _class "hiw-title" ] [ str "Order Confirmed" ]
                    p [ _class "hiw-desc" ] [ str "Your payment is verified and a dedicated logistics coordinator is assigned within 24 hours." ]
                ]
                div [ _class "hiw-step" ] [
                    div [ _class "hiw-num" ] [ str "2" ]
                    p [ _class "hiw-title" ] [ str "Production & Inspection" ]
                    p [ _class "hiw-desc" ] [ str "The aircraft undergoes final assembly checks, cabin configuration, and ICAO airworthiness certification." ]
                ]
                div [ _class "hiw-step" ] [
                    div [ _class "hiw-num" ] [ str "3" ]
                    p [ _class "hiw-title" ] [ str "Delivered to Your Airport" ]
                    p [ _class "hiw-desc" ] [ str "Your aircraft is flown by a delivery crew directly to the ICAO airport code you specified at checkout." ]
                ]
            ]
        ]
    ]

// ==========================================
// SHOPPING CART VIEW
// ==========================================
// Shows all items in the session cart, a live currency converter,
// a running total, and individual checkout links.
let cartView (items: CartItem list) (cartCount: int) (username: string option) =
    // Compute total in USD (using F# Units of Measure)
    let totalUsd = items |> List.sumBy (fun i -> i.PriceUsd)

    // Currency conversions using the HOFs defined in Models.fs
    // We embed the rates as JS constants so the client-side toggle is instant.
    let ratesJs =
        "const rates = { USD: 1.0, EUR: " + string usdToEurRate + ", SGD: " + string usdToSgdRate + ", GBP: " + string usdToGbpRate + " };\n" +
        "const symbols = { USD: '$', EUR: '€', SGD: 'S$', GBP: '£' };\n" +
        "let activeCurrency = 'USD';\n" +
        "function setCurrency(code) {\n" +
        "  activeCurrency = code;\n" +
        "  document.querySelectorAll('.cur-btn').forEach(b => b.classList.toggle('selected', b.dataset.cur === code));\n" +
        "  document.querySelectorAll('[data-usd]').forEach(el => {\n" +
        "    const usd = parseFloat(el.dataset.usd);\n" +
        "    const converted = usd * rates[code];\n" +
        "    el.textContent = symbols[code] + Math.round(converted).toLocaleString();\n" +
        "  });\n" +
        "}\n"

    layout "Shopping Cart | Skypay" cartCount username [
        div [ _class "track-container"; _style "max-width:800px;" ] [

            // ---- Header ----
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [] [ str "Your Cart" ]
                p [ _style "color:#64748b;" ] [
                    str (sprintf "%d item(s) selected" items.Length)
                ]
            ]

            // ---- Currency Switcher ----
            // Uses convertFromUsd (partial application) — rates injected from F# into JS
            div [ _style "margin-bottom:24px;background:#f0fdf4;border:3px solid black;border-radius:12px;padding:16px 20px;box-shadow:4px 4px 0px black;" ] [
                p [ _style "margin:0 0 10px 0;font-weight:900;font-size:0.95rem;" ] [ str "Currency Converter (F# Units of Measure)" ]
                div [ _style "display:flex;gap:10px;flex-wrap:wrap;" ] [
                    for (code, flag) in [("USD","$");("EUR","€");("SGD","S$");("GBP","£")] do
                        button [
                            _class "btn cur-btn"
                            attr "data-cur" code
                            attr "onclick" (sprintf "setCurrency('%s')" code)
                            _style (if code = "USD" then "background:var(--primary);border:3px solid black;" else "background:white;")
                        ] [ str (sprintf "%s %s" flag code) ]
                ]
            ]

            // ---- Empty State ----
            if items.IsEmpty then
                div [ _class "empty-state" ] [
                    div [ _class "emoji" ] [ str "Empty" ]
                    h2 [] [ str "Your cart is empty!" ]
                    p [] [ str "Browse our inventory and add some aircraft." ]
                    a [ _href "/"; _class "btn" ] [ str "← Browse Aircraft" ]
                ]
            else
                // ---- Cart Items ----
                div [ _style "display:flex;flex-direction:column;gap:16px;margin-bottom:28px;" ] (
                    items |> List.map (fun item ->
                        let bgStyle = sprintf "background-image:url('%s');background-size:cover;background-position:center;" item.ImageUrl
                        div [ _style "display:flex;gap:16px;align-items:center;background:white;border:3px solid black;border-radius:12px;padding:16px;box-shadow:4px 4px 0px black;" ] [
                            div [ _style (sprintf "width:90px;height:65px;border-radius:8px;border:2px solid black;flex-shrink:0;%s" bgStyle) ] []
                            div [ _style "flex:1;" ] [
                                p [ _style "margin:0 0 2px 0;font-weight:900;font-size:1.1rem;" ] [ str item.Model ]
                                p [ _style "margin:0;font-size:0.85rem;color:#64748b;" ] [ str item.Manufacturer ]
                                p [ _style "margin:4px 0 0 0;font-weight:900;color:var(--accent);font-size:1.05rem;" ] [
                                    span [ attr "data-usd" (string item.PriceUsd) ] [
                                        str (sprintf "$%s" (item.PriceUsd.ToString("N0")))
                                    ]
                                    str " USD"
                                ]
                            ]
                            div [ _style "display:flex;flex-direction:column;gap:8px;" ] [
                                a [ _href (sprintf "/checkout/%s" item.AircraftId); _class "btn"; _style "font-size:0.8rem;padding:8px 14px;text-align:center;" ] [ str "Buy Now" ]
                                a [ _href (sprintf "/cart/remove/%s" item.AircraftId); _style "font-size:0.8rem;padding:8px 14px;text-align:center;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;font-weight:900;color:#dc2626;text-decoration:none;box-shadow:2px 2px 0px black;" ] [ str "Remove" ]
                            ]
                        ]
                    )
                )

                // ---- Total + Actions ----
                div [ _style "background:var(--primary);border:3px solid black;border-radius:12px;padding:20px 24px;box-shadow:6px 6px 0px black;text-align:center;" ] [
                    p [ _style "margin:0 0 4px 0;font-size:0.9rem;font-weight:700;text-transform:uppercase;letter-spacing:1px;" ] [ str "Cart Total" ]
                    p [ _style "margin:0 0 16px 0;font-size:2.5rem;font-weight:900;" ] [
                        span [ attr "data-usd" (string totalUsd) ] [
                            str (sprintf "$%s" (totalUsd.ToString("N0")))
                        ]
                        str " USD"
                    ]
                    div [ _style "display:flex;gap:12px;justify-content:center;flex-wrap:wrap;" ] [
                        a [ _href "/"; _class "btn"; _style "background:white;" ] [ str "← Continue Shopping" ]
                        a [ _href "/cart/clear"; _style "padding:12px 24px;background:#fee2e2;border:3px solid #dc2626;border-radius:8px;font-weight:900;color:#dc2626;text-decoration:none;box-shadow:4px 4px 0px black;" ] [ str "Clear Cart" ]
                    ]
                ]

            script [] [ rawText ratesJs ]
        ]
    ]

// ==========================================
// LOGIN VIEW
// ==========================================
let loginView (errorMsg: string option) =
    layout "Login | Skypay" 0 None [
        div [ _class "payment-container"; _style "margin-top:60px;" ] [
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [ _style "margin:8px 0 4px;" ] [ str "Welcome Back" ]
                p [ _style "color:#64748b;margin:0;" ] [ str "Log in to your Skypay account" ]
            ]
            match errorMsg with
            | Some msg ->
                div [ _style "padding:12px 16px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;margin-bottom:20px;font-weight:700;color:#dc2626;" ] [
                    str msg
                ]
            | None -> ()
            tag "form" [ attr "action" "/login"; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "username" ] [ str "Username" ]
                    input [ _type "text"; _name "username"; _id "username"; _class "form-input"; _placeholder "Your username"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "password" ] [ str "Password" ]
                    input [ _type "password"; _name "password"; _id "password"; _class "form-input"; _placeholder "Your password"; _required ]
                ]
                button [ _type "submit"; _class "btn"; _style "width:100%;margin-top:8px;" ] [ str "Log In" ]
            ]
            p [ _style "text-align:center;margin-top:20px;color:#64748b;" ] [
                str "No account yet? "
                a [ _href "/register"; _style "font-weight:900;color:var(--accent);" ] [ str "Register here" ]
            ]
        ]
    ]

// ==========================================
// REGISTER VIEW
// ==========================================
let registerView (errorMsg: string option) =
    layout "Register | Skypay" 0 None [
        div [ _class "payment-container"; _style "margin-top:60px;" ] [
            div [ _style "text-align:center;margin-bottom:28px;" ] [
                h1 [ _style "margin:8px 0 4px;" ] [ str "Create Account" ]
                p [ _style "color:#64748b;margin:0;" ] [ str "Join Skypay and start buying aircraft" ]
            ]
            match errorMsg with
            | Some msg ->
                div [ _style "padding:12px 16px;background:#fee2e2;border:2px solid #dc2626;border-radius:8px;box-shadow:2px 2px 0px black;margin-bottom:20px;font-weight:700;color:#dc2626;" ] [
                    str msg
                ]
            | None -> ()
            tag "form" [ attr "action" "/register"; attr "method" "POST" ] [
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "username" ] [ str "Username" ]
                    input [ _type "text"; _name "username"; _id "username"; _class "form-input"; _placeholder "min. 3 characters"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "email" ] [ str "Email Address" ]
                    input [ _type "email"; _name "email"; _id "email"; _class "form-input"; _placeholder "you@gmail.com"; _required ]
                ]
                div [ _class "form-group" ] [
                    label [ _class "form-label"; attr "for" "password" ] [ str "Password" ]
                    input [ _type "password"; _name "password"; _id "password"; _class "form-input"; _placeholder "min. 6 characters"; _required ]
                ]
                button [ _type "submit"; _class "btn"; _style "width:100%;margin-top:8px;background:#3b82f6;color:white;" ] [ str "Create Account" ]
            ]
            p [ _style "text-align:center;margin-top:20px;color:#64748b;" ] [
                str "Already have an account? "
                a [ _href "/login"; _style "font-weight:900;color:var(--accent);" ] [ str "Login here" ]
            ]
        ]
    ]

// ==========================================
// LEASE LISTING VIEW
// ==========================================
let leaseListView (aircrafts: Aircraft list) (leases: LeaseRequest list) (cartCount: int) (username: string option) =
    let countryFlag c =
        match c with
        | "Laos"     -> "🇱🇦"
        | "Cambodia" -> "🇰🇭"
        | "Myanmar"  -> "🇲🇲"
        | "Vietnam"  -> "🇻🇳"
        | "Thailand" -> "🇹🇭"
        | _          -> "🌏"

    let statusLabel s =
        match s with
        | Enquiry          -> ("Enquiry",            "status-Enquiry")
        | OfferSubmitted   -> ("Offer Submitted",    "status-OfferSubmitted")
        | NegotiationOpen  -> ("Negotiating",        "status-NegotiationOpen")
        | ContractSigned   -> ("Contract Signed",    "status-ContractSigned")
        | AircraftOnGround -> ("Aircraft On Ground", "status-AircraftOnGround")
        | Cancelled        -> ("Cancelled",          "status-Cancelled")

    let durationLabel d =
        match d with
        | ShortTerm  n -> sprintf "%d-month Short Lease" n
        | MediumTerm n -> sprintf "%d-month Medium Lease" n
        | LongTerm   n -> sprintf "%d-month Long Lease" n

    layout "Lease Aircraft | Skypay" cartCount username [
        div [ _class "lease-hero" ] [
            h1 [] [ str "On-Demand Aircraft Leasing" ]
            p [] [ str "Flexible leases for Southeast Asian airlines. Lease when you need it. Cancel when you don't." ]
            div [ _class "lease-stats" ] [
                div [ _class "lease-stat" ] [
                    div [ _class "ls-val" ] [ str (string aircrafts.Length) ]
                    div [ _class "ls-label" ] [ str "Aircraft Available" ]
                ]
                div [ _class "lease-stat" ] [
                    div [ _class "ls-val" ] [ str "5" ]
                    div [ _class "ls-label" ] [ str "SEA Countries" ]
                ]
                div [ _class "lease-stat" ] [
                    div [ _class "ls-val" ] [ str "0%" ]
                    div [ _class "ls-label" ] [ str "Cancellation Penalty" ]
                ]
                div [ _class "lease-stat" ] [
                    div [ _class "ls-val" ] [ str "LAK" ]
                    div [ _class "ls-label" ] [ str "Dual Currency Display" ]
                ]
            ]
        ]

        div [ _style "background:#fffbeb;border:1px solid #fbbf24;border-radius:12px;padding:16px 24px;margin-bottom:32px;" ] [
            p [ _style "margin:0 0 4px;font-weight:700;color:#92400e;" ] [ str "Designed for Laos & Southeast Asia" ]
            p [ _style "margin:0;font-size:0.875rem;color:#78350f;" ] [
                str "Unlike Ryanair's rigid ownership model, Skypay Lease is fully flexible. "
                str "Request an aircraft for peak tourism season (Nov-Apr), then cancel after — zero penalties. "
                str "Pricing shown in both USD and LAK (Lao Kip) for local operators."
            ]
        ]

        h2 [ _style "margin-bottom:20px;" ] [ str "Available for Lease" ]
        div [ _class "grid"; _style "margin-bottom:48px;" ] (
            aircrafts |> List.map (fun ac ->
                let monthlyUsd = shortTermLease ac.Price
                let monthlyLak = toLak monthlyUsd
                let rateUsdStr = sprintf "$%s/mo" ((monthlyUsd / 1m<USD>).ToString("N0"))
                let rateLakStr = sprintf "approx. LAK %s/mo" (monthlyLak.ToString("N0"))
                let imgStyle   = sprintf "background-image:url('%s');" ac.ImageUrl
                let leaseUrl   = sprintf "/lease/request/%O" ac.Id
                div [ _class "lease-card" ] [
                    div [ _class "lease-card-img"; _style imgStyle ] [
                        div [ _class "lease-card-badge" ] [ str ac.Manufacturer ]
                    ]
                    div [ _class "lease-card-body" ] [
                        p [ _style "margin:0 0 4px;font-weight:700;font-size:1.05rem;color:var(--navy);" ] [ str ac.Model ]
                        div [ _class "lease-rate-usd" ] [ str rateUsdStr ]
                        div [ _class "lease-rate-lak" ] [ str rateLakStr ]
                        p [ _style "font-size:0.8rem;color:var(--muted);margin:10px 0 14px;flex:1;" ] [
                            str (if ac.Description.Length > 70 then ac.Description.[..69] + "..." else ac.Description)
                        ]
                        div [ _style "display:flex;align-items:center;gap:10px;justify-content:space-between;flex-wrap:wrap;" ] [
                            span [ _class "cancel-badge cancel-yes" ] [ str "Free Cancel" ]
                            a [ _href leaseUrl; _class "btn"; _style "font-size:0.82rem;padding:8px 16px;" ] [ str "Request Lease" ]
                        ]
                    ]
                ]
            )
        )

        h2 [ _style "margin-bottom:20px;" ] [ str "Live Lease Activity — SEA Region" ]
        div [ _style "display:flex;flex-direction:column;gap:12px;margin-bottom:48px;" ] (
            leases |> List.map (fun lr ->
                let (statusText, statusCls) = statusLabel lr.Status
                let flag    = countryFlag lr.Country
                let durText = durationLabel lr.Duration
                let rateStr = sprintf "$%s/mo" ((lr.MonthlyRate / 1m<USD>).ToString("N0"))
                div [ _class "lease-activity-row" ] [
                    div [ _class "lar-flag" ] [ str flag ]
                    div [ _class "lar-body" ] [
                        p [ _class "lar-airline" ] [ str (sprintf "%s — %s" lr.AirlineName lr.AircraftModel) ]
                        p [ _class "lar-detail" ] [ str (sprintf "%s · %s · %s · %s" lr.Airport durText rateStr lr.RequestDate) ]
                    ]
                    span [ _class (sprintf "status-pill %s" statusCls) ] [ str statusText ]
                    if lr.Cancellable then
                        span [ _class "cancel-badge cancel-yes"; _style "flex-shrink:0;" ] [ str "Cancellable" ]
                    else
                        span [ _class "cancel-badge cancel-no"; _style "flex-shrink:0;" ] [ str "Committed" ]
                ]
            )
        )

        div [ _style "background:var(--navy);color:white;border-radius:var(--radius-lg);padding:36px 40px;" ] [
            h2 [ _style "color:var(--gold);margin-bottom:16px;" ] [ str "Why F# Powers This Leasing Platform" ]
            div [ _style "display:grid;grid-template-columns:1fr 1fr;gap:24px;color:rgba(255,255,255,0.82);font-size:0.9rem;" ] [
                div [] [
                    p [ _style "font-weight:700;color:white;margin:0 0 6px;" ] [ str "Units of Measure (LAK / USD)" ]
                    p [ _style "margin:0;font-family:monospace;background:rgba(255,255,255,0.08);padding:10px;border-radius:6px;font-size:0.82rem;" ] [
                        str "[<Measure>] type USD"; br []; str "[<Measure>] type LAK"; br []; str "// Compiler rejects mixing them!"
                    ]
                ]
                div [] [
                    p [ _style "font-weight:700;color:white;margin:0 0 6px;" ] [ str "Discriminated Union (LeaseStatus)" ]
                    p [ _style "margin:0;font-family:monospace;background:rgba(255,255,255,0.08);padding:10px;border-radius:6px;font-size:0.82rem;" ] [
                        str "| Enquiry"; br []; str "| ContractSigned"; br []; str "| Cancelled  // all cases enforced"
                    ]
                ]
                div [] [
                    p [ _style "font-weight:700;color:white;margin:0 0 6px;" ] [ str "Curried HOF (Lease Pricing)" ]
                    p [ _style "margin:0;font-family:monospace;background:rgba(255,255,255,0.08);padding:10px;border-radius:6px;font-size:0.82rem;" ] [
                        str "let leaseMonthlyRate ="; br []; str "  fun dur -> fun price ->"; br []; str "    baseRate * modifier"
                    ]
                ]
                div [] [
                    p [ _style "font-weight:700;color:white;margin:0 0 6px;" ] [ str "Partial Application (Duration Rates)" ]
                    p [ _style "margin:0;font-family:monospace;background:rgba(255,255,255,0.08);padding:10px;border-radius:6px;font-size:0.82rem;" ] [
                        str "let shortTermLease ="; br []; str "  leaseMonthlyRate (ShortTerm 3)"; br []; str "// ready-to-use lambda"
                    ]
                ]
            ]
        ]
    ]

// ==========================================
// LEASE REQUEST FORM VIEW
// ==========================================
let leaseRequestView (aircraft: Aircraft) (submitted: bool) (cartCount: int) (username: string option) =
    let shortRateUsd = shortTermLease  aircraft.Price
    let medRateUsd   = mediumTermLease aircraft.Price
    let longRateUsd  = longTermLease   aircraft.Price
    let shortRateLak = toLak shortRateUsd
    let medRateLak   = toLak medRateUsd
    let longRateLak  = toLak longRateUsd
    let toIntU (v: decimal<USD>) = int (v / 1m<USD>)
    let toIntD (v: decimal)      = int v

    let jsScript =
        "const rates = {" +
        "  short:  { usd: " + string (toIntU shortRateUsd) + ", lak: " + string (toIntD shortRateLak) + ", label: 'Short Term (1-3 months) — +30% premium' }," +
        "  medium: { usd: " + string (toIntU medRateUsd)   + ", lak: " + string (toIntD medRateLak)   + ", label: 'Medium Term (4-12 months) — Standard rate' }," +
        "  long:   { usd: " + string (toIntU longRateUsd)  + ", lak: " + string (toIntD longRateLak)  + ", label: 'Long Term (13-36 months) — 20% discount' }" +
        "};\n" +
        "function selectDuration(key) {\n" +
        "  document.querySelectorAll('.duration-btn').forEach(b => b.classList.toggle('active', b.dataset.key === key));\n" +
        "  document.getElementById('dur-input').value = key;\n" +
        "  const r = rates[key];\n" +
        "  document.getElementById('dur-label').textContent = r.label;\n" +
        "  document.getElementById('rate-usd').textContent = '$' + r.usd.toLocaleString() + '/month';\n" +
        "  document.getElementById('rate-lak').textContent = 'LAK ' + r.lak.toLocaleString() + '/month';\n" +
        "  document.getElementById('price-box').style.display = 'block';\n" +
        "}\n" +
        "selectDuration('short');\n"

    let actionUrl  = sprintf "/lease/request/%O" aircraft.Id
    let priceText  = sprintf "$%s USD" ((aircraft.Price / 1m<USD>).ToString("N0"))
    let fromRate   = sprintf "$%s/month" ((shortRateUsd / 1m<USD>).ToString("N0"))

    layout (sprintf "Lease %s | Skypay" aircraft.Model) cartCount username [
        div [ _class "lease-form-container" ] [
            if submitted then
                div [ _style "text-align:center;padding:60px 20px;" ] [
                    div [ _style "font-size:3rem;margin-bottom:16px;" ] [ str "OK" ]
                    h1 [ _style "color:var(--navy);" ] [ str "Lease Request Submitted!" ]
                    p [ _style "color:var(--muted);font-size:1.05rem;margin-bottom:8px;" ] [ str (sprintf "Your request for the %s has been received." aircraft.Model) ]
                    p [ _style "color:var(--muted);margin-bottom:32px;" ] [ str "A Skypay leasing officer will contact you within 48 hours to discuss terms." ]
                    div [ _style "background:#f0fdf4;border:2px solid #16a34a;border-radius:12px;padding:20px;margin-bottom:32px;text-align:left;" ] [
                        p [ _style "font-weight:700;margin:0 0 8px;" ] [ str "What happens next?" ]
                        p [ _style "margin:4px 0;font-size:0.875rem;color:#475569;" ] [ str "1. Our team reviews your route plan and aircraft suitability." ]
                        p [ _style "margin:4px 0;font-size:0.875rem;color:#475569;" ] [ str "2. A formal lease offer is prepared within 48 hours." ]
                        p [ _style "margin:4px 0;font-size:0.875rem;color:#475569;" ] [ str "3. You can accept, negotiate, or cancel — no commitment yet." ]
                        p [ _style "margin:4px 0;font-size:0.875rem;color:#16a34a;font-weight:600;" ] [ str "4. Aircraft delivered to your airport once contract is signed." ]
                    ]
                    div [ _style "display:flex;gap:12px;justify-content:center;" ] [
                        a [ _href "/lease"; _class "btn" ] [ str "Back to Lease Listings" ]
                        a [ _href "/"; _class "btn"; _style "background:var(--gold);color:var(--navy);" ] [ str "Browse Aircraft to Buy" ]
                    ]
                ]
            else
                div [ _class "lease-aircraft-summary" ] [
                    div [ _style (sprintf "width:100px;height:70px;border-radius:8px;border:2px solid rgba(255,255,255,0.2);background-image:url('%s');background-size:cover;background-position:center;flex-shrink:0;" aircraft.ImageUrl) ] []
                    div [] [
                        p [ _class "las-title" ] [ str aircraft.Model ]
                        p [ _class "las-mfr"   ] [ str aircraft.Manufacturer ]
                        p [ _class "las-price" ] [ str (sprintf "List price: %s · Lease from ~%s" priceText fromRate) ]
                        span [ _class "cancel-badge cancel-yes"; _style "margin-top:8px;display:inline-flex;" ] [ str "Free Cancellation" ]
                    ]
                ]

                h2 [ _style "margin-bottom:6px;" ] [ str "Submit Lease Request" ]
                p [ _style "color:var(--muted);margin-bottom:28px;font-size:0.9rem;" ] [
                    str "Fill in your airline details. A Skypay leasing officer will respond within 48 hours."
                ]

                tag "form" [ attr "action" actionUrl; attr "method" "POST" ] [
                    div [ _style "background:#f8fafc;border:1px solid var(--border);border-radius:12px;padding:24px;margin-bottom:24px;" ] [
                        p [ _style "font-weight:700;font-size:0.875rem;text-transform:uppercase;letter-spacing:0.06em;color:var(--muted);margin:0 0 18px;" ] [ str "Airline Information" ]
                        div [ _class "form-group" ] [
                            label [ _class "form-label" ] [ str "Airline / Operator Name" ]
                            input [ _type "text"; _name "airlineName"; _id "airlineName"; _class "form-input"; _placeholder "e.g. Lao Airlines, Lao Skyway"; _required ]
                        ]
                        div [ _style "display:grid;grid-template-columns:1fr 1fr;gap:16px;" ] [
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Contact Person" ]
                                input [ _type "text"; _name "contactName"; _id "contactName"; _class "form-input"; _placeholder "Full name"; _required ]
                            ]
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Contact Email" ]
                                input [ _type "email"; _name "contactEmail"; _id "contactEmail"; _class "form-input"; _placeholder "ops@airline.la"; _required ]
                            ]
                        ]
                        div [ _style "display:grid;grid-template-columns:1fr 1fr;gap:16px;" ] [
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Country" ]
                                select [ _name "country"; _id "country"; _class "form-input"; _required ] [
                                    option [ _value "Laos"     ] [ str "Laos" ]
                                    option [ _value "Cambodia" ] [ str "Cambodia" ]
                                    option [ _value "Myanmar"  ] [ str "Myanmar" ]
                                    option [ _value "Vietnam"  ] [ str "Vietnam" ]
                                    option [ _value "Thailand" ] [ str "Thailand" ]
                                    option [ _value "Other"    ] [ str "Other" ]
                                ]
                            ]
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Base Airport (ICAO/Name)" ]
                                input [ _type "text"; _name "airport"; _id "airport"; _class "form-input"; _placeholder "e.g. Wattay Intl (VTE)"; _required ]
                            ]
                        ]
                        div [ _class "form-group"; _style "margin-bottom:0;" ] [
                            label [ _class "form-label" ] [ str "Intended Route Plan" ]
                            tag "textarea" [ attr "name" "routePlan"; attr "id" "routePlan"; attr "class" "form-input"; attr "rows" "3"; attr "placeholder" "e.g. VTE-BKK, VTE-HAN — seasonal tourism Nov to Apr 2026"; attr "required" "" ] []
                        ]
                    ]

                    div [ _style "background:#f8fafc;border:1px solid var(--border);border-radius:12px;padding:24px;margin-bottom:24px;" ] [
                        p [ _style "font-weight:700;font-size:0.875rem;text-transform:uppercase;letter-spacing:0.06em;color:var(--muted);margin:0 0 16px;" ] [ str "Lease Duration & Pricing" ]
                        div [ _style "display:flex;gap:10px;flex-wrap:wrap;margin-bottom:16px;" ] [
                            button [ _type "button"; _class "duration-btn active"; attr "data-key" "short";  attr "onclick" "selectDuration('short')"  ] [ str "Short Term" ]
                            button [ _type "button"; _class "duration-btn";        attr "data-key" "medium"; attr "onclick" "selectDuration('medium')" ] [ str "Medium Term" ]
                            button [ _type "button"; _class "duration-btn";        attr "data-key" "long";   attr "onclick" "selectDuration('long')"   ] [ str "Long Term" ]
                        ]
                        input [ _type "hidden"; _name "duration"; _id "dur-input"; _value "short" ]
                        p [ _id "dur-label"; _style "font-size:0.875rem;color:var(--muted);margin:0 0 12px;" ] []
                        div [ _id "price-box"; _class "price-preview-box"; _style "display:none;" ] [
                            p [ _style "margin:0 0 4px;font-size:0.8rem;font-weight:600;color:#166534;text-transform:uppercase;letter-spacing:0.05em;" ] [ str "Estimated Monthly Rate" ]
                            p [ _id "rate-usd"; _style "margin:0;font-size:1.6rem;font-weight:800;color:var(--navy);" ] []
                            p [ _id "rate-lak"; _style "margin:4px 0 0;font-size:0.875rem;color:var(--muted);" ] []
                            p [ _style "margin:12px 0 0;font-size:0.78rem;color:#166534;" ] [
                                str "Rate calculated by F# Lambda Calculus: "
                                code [] [ str "leaseMonthlyRate duration price" ]
                            ]
                        ]
                    ]

                    div [ _style "background:#fffbeb;border:1px solid #fbbf24;border-radius:12px;padding:20px 24px;margin-bottom:28px;display:flex;gap:14px;align-items:flex-start;" ] [
                        div [ _style "font-size:1.4rem;margin-top:2px;" ] [ str "Note" ]
                        div [] [
                            p [ _style "font-weight:700;margin:0 0 4px;color:#92400e;" ] [ str "Flexible Cancellation Guarantee" ]
                            p [ _style "margin:0;font-size:0.875rem;color:#78350f;" ] [
                                str "You can cancel this lease request at any time before the contract is signed — "
                                strong [] [ str "no fees, no penalties" ]
                                str ". Once signed, a 30-day notice period applies. Unlike rigid airline ownership models, we understand that demand in Laos and SEA is seasonal."
                            ]
                        ]
                    ]

                    button [ _type "submit"; _class "btn"; _style "width:100%;padding:14px;font-size:1rem;" ] [ str "Submit Lease Request" ]
                    p [ _style "text-align:center;margin-top:16px;font-size:0.8rem;color:var(--muted);" ] [
                        str "No commitment until contract is signed. A Skypay officer will contact you within 48 hours."
                    ]
                ]
                script [] [ rawText jsScript ]
        ]
    ]

// ==========================================
// CONTACT AGENCY VIEW
// ==========================================
let contactView (submitted: bool) (cartCount: int) (username: string option) =
    layout "Contact Agency | Skypay" cartCount username [
        div [ _class "container"; _style "max-width: 1000px;" ] [
            div [ _class "contact-hero" ] [
                h1 [] [ str "Contact Skypay Agency" ]
                p [] [ str "Get in touch with our leasing officers and sales representatives." ]
            ]

            if submitted then
                div [ _class "contact-form-card contact-success" ] [
                    div [ _class "contact-success-icon" ] [ str "✓" ]
                    h2 [ _style "color: var(--navy); margin-bottom: 12px;" ] [ str "Message Sent Successfully!" ]
                    p [ _style "color: var(--muted); max-width: 400px; margin: 0 auto 24px;" ] [ str "Thank you for reaching out. One of our aviation specialists will review your inquiry and get back to you within 24 business hours." ]
                    a [ _href "/"; _class "btn" ] [ str "Return to Homepage" ]
                ]
            else
                div [ _class "contact-grid" ] [
                    // Left Column - Contact Info
                    div [ _class "contact-info-card" ] [
                        h3 [ _style "margin-bottom: 24px; color: var(--navy);" ] [ str "Corporate Headquarters" ]
                        
                        div [ _class "contact-info-item" ] [
                            div [ _class "ci-icon" ] [ str "📍" ]
                            div [] [
                                p [ _class "ci-label" ] [ str "Office Address" ]
                                p [ _class "ci-value" ] [ str "Vientiane Capital" ]
                                p [ _class "ci-value"; _style "font-size: 0.85rem; color: var(--muted);" ] [ str "Lao PDR" ]
                            ]
                        ]
                        
                        div [ _class "contact-info-item" ] [
                            div [ _class "ci-icon" ] [ str "✉️" ]
                            div [] [
                                p [ _class "ci-label" ] [ str "Email Us" ]
                                p [ _class "ci-value" ] [ str "inquiries@skypay.la" ]
                                p [ _class "ci-value"; _style "font-size: 0.85rem; color: var(--muted);" ] [ str "sales@skypay.la" ]
                            ]
                        ]
                        
                        div [ _class "contact-info-item" ] [
                            div [ _class "ci-icon" ] [ str "📞" ]
                            div [] [
                                p [ _class "ci-label" ] [ str "Call Us" ]
                                p [ _class "ci-value" ] [ str "+856 20 5555 1234" ]
                                p [ _class "ci-value"; _style "font-size: 0.85rem; color: var(--muted);" ] [ str "Mon-Fri, 9:00 AM - 6:00 PM" ]
                            ]
                        ]
                    ]

                    // Right Column - Contact Form
                    div [ _class "contact-form-card" ] [
                        h3 [ _style "margin-bottom: 8px; color: var(--navy);" ] [ str "Send us a message" ]
                        p [ _style "font-size: 0.9rem; color: var(--muted); margin-bottom: 24px;" ] [ str "Have a question about purchasing or leasing? Let us know." ]
                        
                        tag "form" [ attr "action" "/contact"; attr "method" "POST" ] [
                            div [ _style "display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 16px;" ] [
                                div [ _class "form-group"; _style "margin-bottom: 0;" ] [
                                    label [ _class "form-label" ] [ str "Your Name" ]
                                    input [ _type "text"; _name "name"; _class "form-input"; _placeholder "John Doe"; _required ]
                                ]
                                div [ _class "form-group"; _style "margin-bottom: 0;" ] [
                                    label [ _class "form-label" ] [ str "Email Address" ]
                                    input [ _type "email"; _name "email"; _class "form-input"; _placeholder "john@example.com"; _required ]
                                ]
                            ]
                            
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Inquiry Type" ]
                                select [ _name "inquiryType"; _class "form-input"; _required ] [
                                    option [ _value "Sales" ] [ str "Aircraft Sales" ]
                                    option [ _value "Leasing" ] [ str "Aircraft Leasing" ]
                                    option [ _value "Support" ] [ str "General Support" ]
                                    option [ _value "Other" ] [ str "Other" ]
                                ]
                            ]
                            
                            div [ _class "form-group" ] [
                                label [ _class "form-label" ] [ str "Message" ]
                                tag "textarea" [ attr "name" "message"; attr "class" "form-input"; attr "placeholder" "How can we help you?"; attr "required" "" ] []
                            ]
                            
                            button [ _type "submit"; _class "btn"; _style "width: 100%; padding: 12px; font-size: 1rem;" ] [ str "Send Message" ]
                        ]
                    ]
                ]
        ]
    ]
