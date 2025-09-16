import os
import glob
from pathlib import Path

def extract_text_from_pdf(pdf_path):
    """Extract text from PDF using available libraries."""
    try:
        # Try pdfminer3k (Python 3.7 compatible)
        from pdfminer3k.high_level import extract_text
        return extract_text(pdf_path)
    except ImportError:
        try:
            # Try PyPDF2 as fallback
            import PyPDF2
            text = ""
            with open(pdf_path, 'rb') as file:
                pdf_reader = PyPDF2.PdfReader(file)
                for page in pdf_reader.pages:
                    text += page.extract_text() + "\n"
            return text
        except ImportError:
            try:
                # Try pdfplumber as another option
                import pdfplumber
                text = ""
                with pdfplumber.open(pdf_path) as pdf:
                    for page in pdf.pages:
                        page_text = page.extract_text()
                        if page_text:
                            text += page_text + "\n"
                return text
            except ImportError:
                raise ImportError(
                    "No compatible PDF library found. Please install one of:\n"
                    "  pip install pdfminer3k\n"
                    "  pip install PyPDF2\n"
                    "  pip install pdfplumber\n"
                    "  Or upgrade Python to 3.8+ and use: pip install pdfminer.six"
                )

def convert_pdf_to_md(pdf_path, output_dir="converted_md"):
    """Convert a single PDF file to markdown format."""
    try:
        # Extract text from PDF
        text = extract_text_from_pdf(pdf_path)

        # Create output directory if it doesn't exist
        os.makedirs(output_dir, exist_ok=True)

        # Generate output filename
        pdf_name = Path(pdf_path).stem
        md_path = os.path.join(output_dir, f"{pdf_name}.md")

        # Write to markdown file
        with open(md_path, "w", encoding="utf-8") as f:
            f.write(f"# {pdf_name}\n\n")
            f.write("*Converted from PDF*\n\n")
            f.write("---\n\n")
            f.write(text)

        print(f"✓ Converted: {pdf_path} -> {md_path}")
        return True

    except Exception as e:
        print(f"✗ Failed to convert {pdf_path}: {str(e)}")
        return False

def main():
    """Convert all PDF files in the current directory to markdown."""
    current_dir = os.path.dirname(os.path.abspath(__file__))
    pdf_files = glob.glob(os.path.join(current_dir, "*.pdf"))

    if not pdf_files:
        print("No PDF files found in the current directory.")
        return

    print(f"Found {len(pdf_files)} PDF file(s) to convert:")
    for pdf_file in pdf_files:
        print(f"  - {os.path.basename(pdf_file)}")

    print("\nStarting conversion...")
    successful = 0
    failed = 0

    for pdf_file in pdf_files:
        if convert_pdf_to_md(pdf_file):
            successful += 1
        else:
            failed += 1

    print("\nConversion complete!")
    print(f"✓ Successfully converted: {successful}")
    print(f"✗ Failed to convert: {failed}")

if __name__ == "__main__":
    main()
