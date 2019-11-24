using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FakeItEasy;
using FileSender.Dependencies;
using FluentAssertions;
using NUnit.Framework;

namespace FileSender
{
    public class FileSender
    {
        private readonly ICryptographer cryptographer;
        private readonly ISender sender;
        private readonly IRecognizer recognizer;

        public FileSender(
            ICryptographer cryptographer,
            ISender sender,
            IRecognizer recognizer)
        {
            this.cryptographer = cryptographer;
            this.sender = sender;
            this.recognizer = recognizer;
        }

        public Result SendFiles(File[] files, X509Certificate certificate)
        {
            return new Result
            {
                SkippedFiles = files
                    .Where(file => !TrySendFile(file, certificate))
                    .ToArray()
            };
        }

        private bool TrySendFile(File file, X509Certificate certificate)
        {
            Document document;
            if (!recognizer.TryRecognize(file, out document))
                return false;
            if (!CheckFormat(document) || !CheckActual(document))
                return false;
            var signedContent = cryptographer.Sign(document.Content, certificate);
            return sender.TrySend(signedContent);
        }

        private bool CheckFormat(Document document)
        {
            return document.Format == "4.0" ||
                   document.Format == "3.1";
        }

        private bool CheckActual(Document document)
        {
            return document.Created.AddMonths(1) > DateTime.Now;
        }

        public class Result
        {
            public File[] SkippedFiles { get; set; }
        }
    }

    //TODO: реализовать недостающие тесты
    [TestFixture]
    public class FileSender_Should
    {
        private FileSender fileSender;
        private ICryptographer cryptographer;
        private ISender sender;
        private IRecognizer recognizer;

        private readonly X509Certificate certificate = new X509Certificate();
        private File file;
        private byte[] signedContent;


        public Document CreateDocument(string name, byte[] content, DateTime createdAt, string format = "4.0",
            bool sign = true)
        {
            var document = new Document(name, content, createdAt, format);
            A.CallTo(() => recognizer.TryRecognize(file, out document))
                .Returns(true);
            if(sign)
                document = SignDocument(document);
            A.CallTo(() => sender.TrySend(signedContent))
                .Returns(true);
            return document;
        }

        public Document SignDocument(Document document) {
            A.CallTo(() => cryptographer.Sign(document.Content, certificate))
                .Returns(signedContent);
            return document;
        }
        
        [SetUp]
        public void SetUp()
        {
            // Постарайтесь вынести в SetUp всё неспецифическое конфигурирование так,
            // чтобы в конкретных тестах осталась только специфика теста,
            // без конфигурирования "обычного" сценария работы

            file = new File("someFile", new byte[] {1, 2, 3});
            signedContent = new byte[] {1, 7};

            cryptographer = A.Fake<ICryptographer>();
            sender = A.Fake<ISender>();
            recognizer = A.Fake<IRecognizer>();
            fileSender = new FileSender(cryptographer, sender, recognizer);
        }

        [TestCase("4.0")]
        [TestCase("3.1")]
        public void Send_WhenGoodFormat(string format)
        {
            CreateDocument(file.Name, file.Content, DateTime.Now, format);

            fileSender.SendFiles(new[] {file}, certificate)
                .SkippedFiles.Should().BeEmpty();
        }


        [TestCase("3.2")]
        public void Skip_WhenBadFormat(string format)
        {
            var document = CreateDocument(file.Name, file.Content, DateTime.Now, format);
            
            fileSender.SendFiles(new[] {file}, certificate)
                .SkippedFiles.Should().HaveCount(1);
        }

        [TestCase]
        
        public void Skip_WhenOlderThanAMonth() {
            var oldDate = DateTime.Today.AddMonths(-1);
            CreateDocument(file.Name, file.Content, oldDate);

            fileSender.SendFiles(new[] {file}, certificate)
                .SkippedFiles.Should().HaveCount(1);
        }

        [TestCase]
        public void Send_WhenYoungerThanAMonth()
        {
            var oldDate = DateTime.Today.AddDays(-7);
            CreateDocument(file.Name, file.Content, oldDate);

            fileSender.SendFiles(new[] {file}, certificate)
                .SkippedFiles.Should().HaveCount(0);
        }

        
        [TestCase]
        public void Skip_WhenSendFails()
        {
            var document = CreateDocument(file.Name, file.Content, DateTime.Now);
            A.CallTo(() => sender.TrySend(signedContent))
                .Returns(false);
        }

        [TestCase]
        public void Skip_WhenNotRecognized()
        {
            var document = CreateDocument(file.Name, file.Content, DateTime.Now);
            A.CallTo(() => recognizer.TryRecognize(file, out document))
                .Returns(false);
        }
        

        [TestCase]
        public void IndependentlySend_WhenSeveralFilesAndSomeAreInvalid() {
            var document = CreateDocument(file.Name, file.Content, DateTime.Now, sign: false);
            var document2 = CreateDocument(file.Name, file.Content, DateTime.Now);
            var file1 = new File(document.Name, document.Content);
            var file2 = new File(document.Name, document.Content);
            fileSender.SendFiles(new[] {file1, file2}, )
                .SkippedFiles.Should().HaveCount(1);
        }

        [TestCase]
        public void IndependentlySend_WhenSeveralFilesAndSomeCouldNotSend()
        {
            
        }
    }
}
