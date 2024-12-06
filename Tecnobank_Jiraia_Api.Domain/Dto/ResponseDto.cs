namespace Tecnobank_Jiraia_Api.Domain.Dto
{
    public class ResponseDto
    {
        public int status { get; set; }

        public bool isvalid { get; set; }

        public int? idAgrupador { get; set; }

        public string  message { get; set; }
    }
}
