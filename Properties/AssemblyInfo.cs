/*
 *    Copyright 2009-2011 Francesco Tonucci
 * 
 * This file is part of PDFRider.
 * 
 * PDFRider is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 * 
 * PDFRider is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with PDFRider; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 * 
 * 
 * Project page: http://pdfrider.codeplex.com
*/

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// Le informazioni generali relative a un assembly sono controllate dal seguente 
// insieme di attributi. Per modificare le informazioni associate a un assembly
// occorre quindi modificare i valori di questi attributi.
[assembly: AssemblyTitle("PDF Rider")]
[assembly: AssemblyDescription("A GUI for pdftk.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("PDF Rider")]
[assembly: AssemblyCopyright("Copyright © 2009-2011 Francesco Tonucci")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Se si imposta ComVisible su false, i tipi in questo assembly non saranno visibili 
// ai componenti COM. Se è necessario accedere a un tipo in questo assembly da 
// COM, impostare su true l'attributo ComVisible per tale tipo.
[assembly: ComVisible(false)]

//Per iniziare la compilazione delle applicazioni localizzabili, impostare 
//<UICulture>CultureYouAreCodingWith</UICulture> nel file .csproj
//all'interno di un <PropertyGroup>. Ad esempio, se si utilizza l'inglese (Stati Uniti)
//nei file di origine, impostare <UICulture> su en-US. Rimuovere quindi il commento dall'attributo
//NeutralResourceLanguage riportato di seguito. Aggiornare "en-US" nella
//riga sottostante in modo che corrisponda all'impostazione UICulture nel file di progetto.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //dove si trovano i dizionari delle risorse specifiche del tema
    //(in uso se non è possibile trovare una risorsa nella pagina 
    // oppure nei dizionari delle risorse dell'applicazione)
    ResourceDictionaryLocation.SourceAssembly //dove si trova il dizionario delle risorse generiche
    //(in uso se non è possibile trovare una risorsa nella pagina, 
    // nell'applicazione o nei dizionari delle risorse specifiche del tema)
)]


// Le informazioni sulla versione di un assembly sono costituite dai seguenti quattro valori:
//
//      Numero di versione principale
//      Numero di versione secondario 
//      Numero build
//      Revisione
//
// È possibile specificare tutti i valori oppure impostare valori predefiniti per i numeri relativi alla revisione e alla build 
// utilizzando l'asterisco (*) come descritto di seguito:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.6.4")]
