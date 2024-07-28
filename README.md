Competent text-recognition in [Tesseract OCR](https://en.wikipedia.org/wiki/Tesseract_(software)), boosted with large language model LLM prompting. Reads a folder with your documents and images, saves the text, cleans it with a model of choice and optionally [answers questions about the contents](https://github.com/plastic-plant/howabout).

This repository contains source code in C# for a small toolkit to read text from scanned documents, saved to PNG, JPEG and PDF. You can switch out tools and models to improve the quality of the output.


## Example

Run __Tessa__ over a folder with your documents and images in the `input` folder and observe results in the `output` folder. There are some example documents to get you started. By default, runs with Tesseract and English language. You can adapt for that, like I did for Dutch and German language.

```shell

tessa.exe
tessa.exe config

tessa.exe ocr
tessa.exe ocr --in file.png --out file.txt
tessa.exe ocr --in "C:\path\to\your\folder"

Progress:  10%   File 1: example-1.png
Progress:  30%   File 2: example-2.png
Progress:  50%   File 3: example-3.pdf
Progress:  60%   File 3: example-3.pdf
Progress:  70%   File 4: example-4.pdf
Progress: 100%   File 5: example-5.jpg
```
---
```
Tessa
│
├───input
│       example-1.png
│       example-2.png
│       example-3.pdf
│       example-4.pdf
│       example-5.jpg
│
├───logs
│       log.txt
│
├───tessdata
│       eng.traineddata
│       nld.traineddata
│
├───models
│       florence-2
│
└───ouput
        example-1.ocr.txt
        example-1.prompt.txt
        ...
```


Defaults don't require additional downloads, but you can configure and retrieve lexicons and models of choice with `tessa config` and `tessa download`, then run again to verify if quality improves. A quick override for the folder locations, model and language is available with command-line options too.


## Use case

Processing an archive of documents covering 10.000 pages of printed and handwritten documents since 1995, the results for text-recogntion with good old Tesseract were, ...ehm, 'less than optimal'. Scanned in low-resolution, saved in various formats and languages, texts in columns and tables, extracting handwritten notes: that just needs a lot of pre-processing.

Instead of putting a lot of time pre-processing input up front, I tried out a couple of LLM models to improve the quality of the output as post-processing. Using the extracted text for keyword and subject search, I'm most interested in the context of the text over a clean extraction. Large language models are excellent for this. Please have a look at this tool. Running some tests with vision models and for OCR and LLM prompting did do wonders for my results and might be helpful to you too.
 

## Features

- Read text from images and PDF-documents with Tesseract OCR and Florence-2 OCR.
- Clean up text by prompting large language models.
- Download Tesseract languages, Florence models and LLM weights as needed.
- Plug in models from Hugging Face through OpenAI API (LM studio, Jan.ai, etc.)
- Configure appsettings in menu, JSON file and override with command-line options.

## Background

Getting some clean text out of my collection was a challenge, and still is. Most of the documents that I had to process are in Dutch language, with various fonts, skews and handwriting styles in horrible quality. Tacky input that Tesseract OCR often is not fully up to. We have more tooling available to us though.

More recent we have models and hardware available to help out interpret and predict language constructs, then auto-correct broken sentences formed from glyphs fast. After decades of matrix matching and feature extraction, we now have the ability to run extracted text through huge sets of markov chains with large language models for error correction. And we can experiment with new vision models that advances and replaces the old OCR pipeline for preprocessing, lexicon matching, etc. Here's a shot at doing the job with what's available today and that looks so much better than a couple of years ago.

Starting out as a wrapper around Tesseract OCR and LLMs to improve reading text from images and documents, we recently have new vision models available that do an awesome job recognising handwritten text. Over time, this project helped me apply new models, optimise and test the ouput of character recognition to extract text and summarise given content for a search engine. While there's a lot of inefficient over-processing in an approach like this, modern hardware forgives us. It allows us to simply chain together toolsets and have results that matter in a way that was not possible before. Getting things done.


## Installation

Download the repository and build the project in Visual Studio. I dropped builds for Windows and Linux in `Releases`, but you should consider this project mostly as a continuous effort trying out models and techniques than a shrinkwrapped release.


## Usage

Type `tessa`, `tessa -h` or `tessa <command> --help` for some help information.

```shell
USAGE:
    tessa [OPTIONS] <COMMAND>

EXAMPLES:
    tessa config
    tessa config --settings tessa.settings.json
    tessa download llm
    tessa download llm https://huggingface.co/...gguf
    tessa download tessdata

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    config                   Configures tessa.settings.json
    download <model-type>    Downloads language models for OCR and LLM prompting
    ocr                      Reads text from images and documents
tessa.exe config

tessa.exe --input "C:\path\to\your\folder" --output "C:\path\to\your\output" --model "t5-base" --questions
```
---

## Configuration

Run `tessa config` to configure the settings for the tool. File `tessa.settings.json` hold the workflow configuration with options for OCR and LLM post processing. 

```shell
Welcome to tessa config. I'm here to help you update settings in tessa.settings.json.


> 1. Change the path where log files are written.
  2. Change the path where images and documents are read.
  3. Change the path where text files after OCR are written.
  4. Change the path where Tesseract tessdata models are stored.
  5. Change the path where Large language models are stored.
  6. Change the OCR engine selected: Tesseract
  7. Change the OCR language models: eng
  8. Change the LLM prompting model: lmstudio
  9. Change the cleanup strategy and prompt to optimize text readouts.
  Q. Quit configuration
```

File `tessa.registry.json` holds a list of options and models that can be downloaded and used in the workflow. You can expand on that as new models come available. For *Tesseract OCR*, you typically find a list of tessdata downloaded from GitHub, stored in folder `tessdata` and set through tessa config. For other OCR, vision models and text LLMs, there models listed that you can download from Hugging Face.

Alternatively, you may like to configure OpenAI API or compatible providers like Ollama, LM Studio and Jan.ai to serve the model. Using an online provider like OpenAI offers excellent quality for a low price, that is something to consider. Just add a small amount to your account, drop in an API key and experiment with options.


## Download Tesseract languages

Run `tessa download tessdata` for a menu or specifiy command-line which languages you would like to retrieve from GitHub.

```shell
Which Tesseract language model would you like to download?

> [ ] Arabic
  [ ] Armenian
  [ ] Bengali
  [ ] Canadian_Aboriginal
  [ ] Cherokee
  [ ] Cyrillic
  [ ] Devanagari
  [ ] Ethiopic
  [ ] Fraktur
  [ ] Georgian
  [ ] Greek
  [ ] Gujarati
  [ ] Gurmukhi
  [ ] Hangul
  [ ] Hangul_vert
  [ ] HanS
  [ ] HanS_vert
  [ ] HanT
  [ ] HanT_vert
  [ ] Hebrew

(Move up and down to scroll through more trained models.)
(Press <space> to toggle a model, <enter> to start download)
```

As part of a scripted setup, you may prefer to download [Tesseract languages](https://tesseract-ocr.github.io/tessdoc/Data-Files-in-different-versions.html) by their lang-codes. Seperate multiple languages with a plus + as is common for Tesseract configuration (e.g. `eng+deu+fra+ita`). The models are downloaded to `/tessdata` folder.

```shell
tessa download tessdata ita       # Downloads Italian language set
tessa download tessdata afr+nld   # Downloads Afrikaans and Dutch (Nederlands)

afr download ---------------------------------------- 100%
nld download ---------------------------------------- 100%
```


## Download large language models

Run `tessa download llm` for a prompt to specify a model to retrieve from Hugging Face. A default is given as an example to get you setup. Or instead specify the model URL directly as an option. These models usually come in GGUF format and are downloaded to `/models` folder. We use these for text prompting to reduce errors from OCR output.

```shell
tessa download llm
tessa download llm <url>
tessa download llm https://huggingface.co/NousResearch/Hermes-2-Pro-Mistral-7B-GGUF/blob/main/Hermes-2-Pro-Mistral-7B.Q4_K_M.gguf

What url should I download the large language model from?
You can find LLMs at website huggingface.co (.gguf)
(https://huggingface.co/.../main/model.gguf)
```

Run `tessa download llm florence-2` to download Microsoft Florence-2 vision model from [Curiosity](https://github.com/curiosity-ai/florence2-sharp/blob/main/Florence2/Downloader/FlorenceModelDownloader.cs). The model was converted to ONNX format, comes in four files and is downloaded to `/models/florence-2` folder. We use Florence for OCR prompting as an alternative to Tesseract.

```shell
tessa download llm florence-2

Downloading ----------------------------------------  100%
Downloading ----------------------------------------  68%
Downloading ----------------------------------------  14%
Downloading ----------------------------------------  0%

Florence-2 model is already downloaded and ready for use.
```