#pragma once
#include <msclr/marshal_cppstd.h>
namespace Project15 {

    using namespace System;
    using namespace System::ComponentModel;
    using namespace System::Collections;
    using namespace System::Windows::Forms;
    using namespace System::Data;
    using namespace System::Drawing;

    /// <summary>
    /// —водка дл€ MyForm
    /// </summary>
    public ref class MyForm : public System::Windows::Forms::Form
    {
    public:
        MyForm(void)
        {
            InitializeComponent();
            //
            //TODO: добавьте код конструктора
            //
        }

    protected:
        /// <summary>
        /// ќсвободить все используемые ресурсы.
        /// </summary>
        ~MyForm()
        {
            if (components)
            {
                delete components;
            }
        }
    private: System::Windows::Forms::TextBox^ InputBox;
    private: System::Windows::Forms::TextBox^ OutputBox;
    private: System::Windows::Forms::Button^ b1;
    protected:



    protected:

    private:
        /// <summary>
        /// ќб€зательна€ переменна€ конструктора.
        /// </summary>
        System::ComponentModel::Container^ components;

#pragma region Windows Form Designer generated code
        /// <summary>
        /// “ребуемый метод дл€ поддержки конструктора Ч не измен€йте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        void InitializeComponent(void)
        {
            this->InputBox = (gcnew System::Windows::Forms::TextBox());
            this->OutputBox = (gcnew System::Windows::Forms::TextBox());
            this->b1 = (gcnew System::Windows::Forms::Button());
            this->SuspendLayout();
            // 
            // InputBox
            // 
            this->InputBox->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 30, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
                static_cast<System::Byte>(204)));
            this->InputBox->Location = System::Drawing::Point(12, 12);
            this->InputBox->Multiline = true;
            this->InputBox->Name = L"InputBox";
            this->InputBox->Size = System::Drawing::Size(1004, 172);
            this->InputBox->TabIndex = 0;
            // 
            // OutputBox
            // 
            this->OutputBox->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 30, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
                static_cast<System::Byte>(204)));
            this->OutputBox->Location = System::Drawing::Point(12, 279);
            this->OutputBox->Multiline = true;
            this->OutputBox->Name = L"OutputBox";
            this->OutputBox->ReadOnly = true;
            this->OutputBox->Size = System::Drawing::Size(1004, 265);
            this->OutputBox->TabIndex = 1;
            // 
            // b1
            // 
            this->b1->Font = (gcnew System::Drawing::Font(L"Microsoft Sans Serif", 30, System::Drawing::FontStyle::Regular, System::Drawing::GraphicsUnit::Point,
                static_cast<System::Byte>(204)));
            this->b1->Location = System::Drawing::Point(420, 190);
            this->b1->Name = L"b1";
            this->b1->Size = System::Drawing::Size(181, 83);
            this->b1->TabIndex = 2;
            this->b1->Text = L"encrypt";
            this->b1->UseVisualStyleBackColor = true;
            // 
            // MyForm
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(6, 13);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->ClientSize = System::Drawing::Size(1028, 556);
            this->Controls->Add(this->b1);
            this->Controls->Add(this->OutputBox);
            this->Controls->Add(this->InputBox);
            this->Name = L"MyForm";
            this->Text = L"MyForm";
            this->ResumeLayout(false);
            this->PerformLayout();

        }
#pragma endregion
    };
}