namespace Glitch9.IO.RESTApi
{
    /*  Korean Guides

        [Stream 데이터 처리]
        Chunk 단위 처리: Stream 데이터는 보통 chunk 단위로 도착합니다. 각 chunk를 개별적으로 처리하거나, 여러 chunk를 결합하여 필요한 데이터 구조를 형성할 수 있습니다.
        비동기 처리: 대부분의 프로그래밍 언어와 라이브러리에서는 Stream 데이터를 비동기적으로 처리할 수 있는 기능을 제공합니다. 이를 통해 데이터가 도착하는 대로 즉시 처리할 수 있습니다.
        객체 매핑: Stream으로부터 도착하는 데이터 조각을 객체로 변환하기 위해서는, 데이터의 구조를 미리 알고 있거나, 동적으로 구조를 해석할 수 있는 로직이 필요합니다. 
        예를 들어, JSON 형태의 데이터가 Stream으로 전송된다면, 각 JSON 조각을 해당하는 객체 모델로 직렬화할 수 있습니다.

        [Server-Sent Events (SSE) 처리]
        이벤트 리스너 구성: SSE는 특정 이벤트에 대한 구독 방식으로 작동합니다. 클라이언트는 서버로부터 이벤트를 수신하기 위한 리스너를 구성해야 하며, 도착하는 각 이벤트를 적절한 객체로 매핑해야 합니다.
        이벤트 스트림 파싱: 도착하는 SSE 메시지는 일반적으로 "event"와 "data" 필드를 포함합니다. 클라이언트는 이 데이터를 파싱하여, 이벤트 유형에 따라 다른 객체로 변환할 수 있습니다.
        동적 객체 매핑: SSE 데이터는 일반적으로 JSON 형식을 취합니다. 이를 통해, 도착하는 데이터를 동적으로 해당 JSON 구조에 맞는 객체로 직렬화할 수 있습니다.
    */
    /*  English Guides

        [Handling Stream Data]
        - Chunk-Based Processing: Stream data is often received in chunks. Each chunk can be processed individually or combined to form the required data structure.
        - Asynchronous Processing: Most programming languages and libraries offer features for asynchronously processing stream data, allowing for immediate handling as data is received.
        - Object Mapping: To transform data fragments received from a stream into objects, an understanding of the data structure or a mechanism for dynamic interpretation is required.
        For instance, if data transmitted over the stream is in JSON format, each JSON fragment can be serialized into its corresponding object model.

        [Handling Server-Sent Events (SSE)]
        - Event Listener Configuration: SSE operates on a subscription model for specific events. The client must set up listeners to receive events from the server and map each arriving event to the appropriate object.
        - Event Stream Parsing: Incoming SSE messages typically include "event" and "data" fields. Clients can parse this data to transform the event type into different objects.
        - Dynamic Object Mapping: SSE data often takes the form of JSON. This allows for the dynamic serialization of incoming data into objects corresponding to the JSON structure.
    */

    public enum StreamMode
    {
        NoStream,

        /// <summary>
        /// Text(string) stream, json stream or SSE
        /// </summary>
        TextStream,

        /// <summary>
        /// binary(byte[]) stream
        /// </summary>
        BinaryStream,
    }
}